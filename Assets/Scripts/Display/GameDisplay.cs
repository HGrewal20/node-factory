using UnityEngine;

// Draws whatever is currently placed on the map, as colored boxes.
// This is Step 1 of the UI: just drawing. Clicking/placing comes in Step 2.
public class GameDisplay : MonoBehaviour
{
    // A single white pixel we can tint any color and stretch into a rectangle.
    private Texture2D pixel;

    // A soft, rounded, blurred-edge square used for empty grid cells.
    // Generated once in Start() instead of imported, since it's just
    // a gradient - no need for an actual image file.
    private Texture2D emptyCellTexture;

    void Awake()
    {
        // GameEngine is the script that actually advances the simulation
        // (ticks machines, timers, etc). Nothing was attaching it anywhere yet,
        // so we add it here in code instead of needing a second manual step.
        if ( GetComponent<GameEngine>() == null )
            gameObject.AddComponent<GameEngine>();
    }

    void Start()
    {
        // Level no longer auto-starts here. GameMenu's "Start Game" button
        // calls GameData.INSTANCE.LevelStart(...) once you click it.
        pixel = new Texture2D(1, 1);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();

        emptyCellTexture = MakeSoftSquareTexture(64);
    }

    void OnGUI()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() || GameMenu.IsPaused )
            return;

        MapView mapView = data.map.mapView;

        // Which grid cells are currently visible on screen (min/max are inclusive).
        RectInt visible = new RectInt(
            mapView.XMin,
            mapView.YMin,
            mapView.XMax - mapView.XMin + 1,
            mapView.YMax - mapView.YMin + 1
        );

        // Every machine touching that area, with no duplicates even though
        // a big machine can occupy many cells.
        Machine[] machines = data.map.mapMachines.GetArray(visible);

        DrawEmptyCells(mapView, visible, data.map.mapMachines);

        foreach (Machine machine in machines)
        {
            DrawMachine(mapView, machine);
            DrawNodes  (mapView, machine);
        }
    }

    // Draws every empty cell in view as a small square, inset by 1 pixel
    // on each side so you can see the grid lines between cells. This is
    // mainly so scrolling/zooming has something to look at everywhere,
    // not just where machines are.
    private void DrawEmptyCells(MapView mapView, RectInt visible, MapMachines mapMachines)
    {
        GUI.color = new Color(0.55f, 0.55f, 0.55f);

        for (int x = visible.xMin; x < visible.xMax; x++)
        {
            for (int y = visible.yMin; y < visible.yMax; y++)
            {
                if ( mapMachines.Get(x, y) != null )
                    continue; // a machine already draws this cell

                Rect gridRectF = new Rect(x, y, 1, 1);
                Rect screenRect = mapView.GridToScreen(gridRectF);

                screenRect.x      += 1;
                screenRect.y      += 1;
                screenRect.width  -= 2;
                screenRect.height -= 2;

                if (screenRect.width <= 0 || screenRect.height <= 0)
                    continue; // zoomed out too far for this to show up

                GUI.DrawTexture(screenRect, emptyCellTexture);
            }
        }
    }

    // Builds a soft, rounded-corner square with a blurred edge, baked into
    // a texture once so we're not doing this math every frame. Every pixel
    // gets an alpha based on how far it is from a rounded-square outline -
    // 1 well inside the shape, fading to 0 past its blurry edge.
    private Texture2D MakeSoftSquareTexture(int size)
    {
        Texture2D tex = new Texture2D(size, size);
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        float half   = size * 0.5f;
        float radius = half * 0.55f; // how rounded the corners are
        float blur   = half * 0.18f; // how wide the soft edge is

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float px = (x + 0.5f) - half;
                float py = (y + 0.5f) - half;

                float dist  = RoundedSquareDistance(px, py, half - blur, radius);
                float alpha = 1f - Mathf.Clamp01((dist + blur) / (blur * 2f));

                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        return tex;
    }

    // Signed distance from (px, py) to a square of half-size "halfSize"
    // with rounded corners of radius "radius". Negative = inside the shape.
    // (Standard rounded-box distance formula.)
    private float RoundedSquareDistance(float px, float py, float halfSize, float radius)
    {
        float qx = Mathf.Abs(px) - halfSize + radius;
        float qy = Mathf.Abs(py) - halfSize + radius;

        float outsideDist = Mathf.Sqrt(Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f)
                                      + Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f));
        float insideDist  = Mathf.Min(Mathf.Max(qx, qy), 0f);

        return outsideDist + insideDist - radius;
    }

    private void DrawMachine(MapView mapView, Machine machine)
    {
        RectInt gridRect = machine.Rect;

        // MapView.GridToScreen expects a float Rect, so convert first.
        Rect gridRectF = new Rect(gridRect.xMin, gridRect.yMin, gridRect.width, gridRect.height);
        Rect screenRect = mapView.GridToScreen(gridRectF);

        GUI.color = ColorFor(machine);
        GUI.DrawTexture(screenRect, pixel);
    }

    // Draws a small marker on every enabled node so you can see exactly
    // where to click to connect it. Black = not connected. Green = connected.
    private void DrawNodes(MapView mapView, Machine machine)
    {
        DrawNodeList(mapView, machine, machine.inputs );
        DrawNodeList(mapView, machine, machine.outputs);
    }
    private void DrawNodeList(MapView mapView, Machine machine, MachineNodes nodes)
    {
        int count = nodes.Count();
        for (int i = 0; i < count; i++)
        {
            MachineNode node = nodes.Get(i);
            if ( !node.IsEnabled )
                continue; // Disabled nodes don't exist visually. See Claude Readme.

            Vector2Int nodePos = machine.Pos + node.Offset;

            Rect gridRectF = new Rect(nodePos.x, nodePos.y, 1, 1);
            Rect screenRect = mapView.GridToScreen(gridRectF);

            GUI.color = node.IsConnected() ? Color.green : Color.black;
            GUI.DrawTexture(screenRect, pixel);
        }
    }

    private Color ColorFor(Machine machine)
    {
        if (machine is MachineWire)       return new Color(0.55f, 0.55f, 0.55f); // gray
        if (machine is MachineDelivery)   return new Color(0.90f, 0.70f, 0.20f); // gold
        if (machine is MachineProcessor)  return new Color(0.30f, 0.60f, 0.90f); // blue

        return Color.white; // flippers, junctions, mergers, splitters - not placed yet
    }
}
