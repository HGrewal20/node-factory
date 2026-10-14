using UnityEngine;
using UnityEngine.UI;

// Draws The Visible Map Each Frame
[DefaultExecutionOrder(100)]
public class DisplayGame : MonoBehaviour
{
    // Member Variables
    [SerializeField] private MenuController menuController;
    [SerializeField] private RawImage       mapImage;
                     private DisplayImage   display;

    private void Awake()
    {
        display = new DisplayImage();

        mapImage.raycastTarget  = false;
        mapImage.color          = Color.white;
        mapImage.transform.SetAsFirstSibling();
    }

    private void Update()
    {
        GameData data = GameData.INSTANCE;

        bool isPlaying = data.IsPlaying() && !data.IsPaused && !menuController.IsGameOverShowing;

        mapImage.enabled = isPlaying;
        if (!isPlaying)
            return;

        MapView           view     = data.map.mapView;
        view.GameTick();
        DisplayGridImages images   = DisplayGridImages.Get(view.Pixels);
        MapMachines       machines = data.map.mapMachines;

        display.Begin(view);

        RectTransform rect  = mapImage.rectTransform;
        float         scale = mapImage.canvas.scaleFactor;

        rect.sizeDelta = new Vector2(
            view.ImageWidth  / scale,
            view.ImageHeight / scale
        );

        rect.anchoredPosition = new Vector2(
            (view.ImageX + view.ImageWidth * 0.5f - Screen.width * 0.5f) / scale,
            (Screen.height * 0.5f - view.ImageY - view.ImageHeight * 0.5f) / scale
        );

        for (int y = view.Y0; y < view.Y1; y++)
            for (int x = view.X0; x < view.X1; x++)
            {
                if (x < 0 || y < 0 || x >= data.map.mapSize.Width || y >= data.map.mapSize.Height)
                    continue;

                Machine machine = machines.Get(x, y);
                     if (machine == null       )
                {
                    display.Draw(images.Empty, x, y);
                }
                else if (machine is MachineWire wire)
                {
                    display.Draw(images.GetWire(wire.Direction), x, y);
                }
                else
                {
                    MachineNode node = machine.GetNode(new Vector2Int(x, y));
                    if (node != null && node.IsEnabled)
                    {
                        byte direction = node.Direction();
                        bool isEmpty   = node.IsEmpty  ();

                        DisplayGridImage image = images.GetNode(direction, isEmpty);
                        display.Draw(image, x, y);
                    }
                    else
                    {
                        display.Draw(images.MachineEmpty, x, y);
                    }  
                }
            }

        display.Apply();

        if (mapImage.texture != display.Texture)
            mapImage.texture = display.Texture;
    }

    private void OnDestroy()
    {
        display.Dispose();
    }
}