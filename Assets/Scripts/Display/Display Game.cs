using UnityEngine;
using UnityEngine.UI;

// Draws The Visible Map Each Frame
[DefaultExecutionOrder(100)]
public class DisplayGame : MonoBehaviour
{
    // Member Variables
    [SerializeField] private DisplayInteraction interaction;
    [SerializeField] private MenuController     menuController;
    [SerializeField] private RawImage           mapImage;
                     private DisplayImage       display;
                     
    private void Awake()
    {
        display = new DisplayImage();

        mapImage.raycastTarget  = false;
        mapImage.color          = Color.white;
        mapImage.transform.SetAsFirstSibling();
    }

    private void Update()
    {
        // Block If Not Playing
        GameData data = GameData.INSTANCE;

        bool isPlaying = data.IsPlaying() && !data.IsPaused && !menuController.IsGameOverShowing;

        mapImage.enabled = isPlaying;
        if (!isPlaying)
            return;

        // Get Information
        MapView           view     = data.map.mapView;
        view.GameTick();
        DisplayGridImages images   = DisplayGridImages.Get(view.Pixels);
        MapMachines       machines = data.map.mapMachines;

        // Draw - Begin
        display ??= new DisplayImage();
        display.Begin(view);

        // Set Image Position
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

        // Draw - Grid
        for (int y = view.Y0; y < view.Y1; y++)
            for (int x = view.X0; x < view.X1; x++)
            {
                if (x < 0 || y < 0 || x >= data.map.mapSize.Width || y >= data.map.mapSize.Height)
                    continue;

                Machine machine = machines.Get(x, y);
                     if (machine == null            )
                {   // Empty Grid
                    display.Draw(images.Empty, x, y);
                }
                else if (machine is MachineWire wire)
                {   // Wire
                    display.Draw(images.GetWire(wire.Direction), x, y);
                }
                else
                {   // Machine
                    MachineNode node = machine.GetNode(new Vector2Int(x, y));
                    if (node != null && node.IsEnabled)                 // Node
                    {
                        byte direction = node.Direction();
                        bool isEmpty   = node.IsEmpty  ();

                        DisplayGridImage image = images.GetNode(direction, isEmpty);
                        display.Draw(image, x, y);
                    }
                    else                                                // Machine Empty
                    {
                        display.Draw(images.MachineEmpty, x, y);
                    }  
                }
            }

        // Draw - Preview
        if (interaction.PreviewVisible)
        {
            Vector2Int pos  = interaction.PreviewPosition;
            Vector2Int size = interaction.PreviewSize;

            // Clip To Visible Viewport And Map Boundaries
            int x0 = Mathf.Max(pos.x, view.X0, 0);
            int y0 = Mathf.Max(pos.y, view.Y0, 0);
            int x1 = Mathf.Min(pos.x + size.x, view.X1, data.map.mapSize.Width);
            int y1 = Mathf.Min(pos.y + size.y, view.Y1, data.map.mapSize.Height);

            // Draw Machine Ghost
            Machine preview = interaction.PreviewMachine;

            for (int y = y0; y < y1; y++)
                for (int x = x0; x < x1; x++)
                {
                    MachineNode node = preview.GetNode(new Vector2Int(x, y));
                    if (node != null && node.IsEnabled)
                    {
                        DisplayGridImage image = images.GetNode(node.Direction(), node.IsEmpty());
                        display.Draw(image, x, y, 0.5f);
                    }
                    else
                    {
                        display.Draw(images.MachineEmpty, x, y, 0.5f);
                    }
                }

            // Draw Invalid Overlay
            if (!interaction.PreviewValid)
                for (int y = y0; y < y1; y++)
                    for (int x = x0; x < x1; x++)
                        display.Draw(images.MachineInvalid, x, y, 0.5f);
        }

        // Draw - Finish
        display.Apply();

        // Push Texture If Needed
        if (mapImage.texture != display.Texture)
            mapImage.texture = display.Texture;
    }

    private void OnDestroy()
    {
        display?.Dispose();
    }
}