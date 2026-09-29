using UnityEngine;

// Draws whatever is currently placed on the map.
// This is Step 1 of the UI: just drawing. Clicking/placing comes in Step 2.
public class GameDisplay : MonoBehaviour
{
    // Soft, rounded, blurred-edge squares, generated once in Start() instead
    // of imported, since they're just gradients - no need for actual image
    // files. Same shape function, different roundness/blur/size per use:
    //   emptyCellTexture - big soft blob for empty grid cells
    //   machineTexture   - tighter, less blurry box for placed machines
    //   nodeTexture      - small, almost-circular dot for node markers
    private Texture2D emptyCellTexture;
    private Texture2D machineTexture;
    private Texture2D nodeTexture;

    private GameInteraction interaction;

    void Awake()
    {
        // GameEngine is the script that actually advances the simulation
        // (ticks machines, timers, etc). Nothing was attaching it anywhere yet,
        // so we add it here in code instead of needing a second manual step.
        if ( GetComponent<GameEngine>() == null )
            gameObject.AddComponent<GameEngine>();

        interaction = GetComponent<GameInteraction>();
    }

    void Start()
    {
        // Level no longer auto-starts here. GameMenu's "Start Game" button
        // calls GameData.INSTANCE.LevelStart(...) once you click it.
        emptyCellTexture = MakeSoftSquareTexture(64, 0.55f, 0.18f);
        machineTexture   = MakeSoftSquareTexture(64, 0.30f, 0.08f);
        nodeTexture      = MakeSoftSquareTexture(32, 0.95f, 0.14f);
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

        // Drawn last on purpose, so the hint text always ends up on top
        // of the grid instead of underneath it.
        if (interaction != null)
            interaction.DrawHint();
    }

    // Draws every empty cell in view as a soft blob, inset by 1 pixel on
    // each side so you can see the grid lines between cells. This is
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

                Rect screenRect = CellToScreen(mapView, x, y, 1);

                if (screenRect.width <= 0 || screenRect.height <= 0)
                    continue; // zoomed out too far for this to show up

                GUI.DrawTexture(screenRect, emptyCellTexture);
            }
        }
    }

    private void DrawMachine(MapView mapView, Machine machine)
    {
        RectInt gridRect = machine.Rect;
        Rect screenRect = CellToScreen(mapView, gridRect.xMin, gridRect.yMin, gridRect.width, gridRect.height);

        // A faint drop shadow, offset a few pixels down-right, so machines
        // look like they're sitting slightly above the grid instead of
        // being painted flat onto it.
        Rect shadowRect = screenRect;
        shadowRect.x += 3;
        shadowRect.y += 3;
        GUI.color = new Color(0f, 0f, 0f, 0.35f);
        GUI.DrawTexture(shadowRect, machineTexture);

        GUI.color = ColorFor(machine);
        GUI.DrawTexture(screenRect, machineTexture);
    }

    // Draws a small dot on every enabled node so you can see exactly
    // where to click to connect it. Dark = not connected. Green = connected.
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
            Rect screenRect = CellToScreen(mapView, nodePos.x, nodePos.y, 1);

            // Shrink to a small dot centered in the cell, rather than
            // filling the whole thing.
            float insetX = screenRect.width  * 0.28f;
            float insetY = screenRect.height * 0.28f;
            screenRect.x      += insetX;
            screenRect.y      += insetY;
            screenRect.width  -= insetX * 2f;
            screenRect.height -= insetY * 2f;

            GUI.color = node.IsConnected()
                ? new Color(0.35f, 0.85f, 0.45f)   // connected: soft green
                : new Color(0.15f, 0.15f, 0.15f);  // not connected: dark dot

            GUI.DrawTexture(screenRect, nodeTexture);
        }
    }

    private Color ColorFor(Machine machine)
    {
        if (machine is MachineWire)       return new Color(0.55f, 0.55f, 0.58f); // gray
        if (machine is MachineDelivery)   return new Color(0.95f, 0.75f, 0.30f); // gold
        if (machine is MachineProcessor)  return new Color(0.35f, 0.65f, 0.95f); // blue

        return new Color(0.90f, 0.90f, 0.92f); // flippers, junctions, mergers, splitters - not placed yet
    }

    // Converts a grid-space rectangle (bottom-left cell + size in cells)
    // to a screen Rect through the current MapView.
    private Rect CellToScreen(MapView mapView, int gridX, int gridY, int width, int height = -1)
    {
        if (height < 0)
            height = width;

        Rect gridRectF = new Rect(gridX, gridY, width, height);
        return mapView.GridToScreen(gridRectF);
    }

    // Builds a soft, rounded-corner square with a blurred edge, baked into
    // a texture once so we're not doing this math every frame. Every pixel
    // gets an alpha based on how far it is from a rounded-square outline -
    // 1 well inside the shape, fading to 0 past its blurry edge.
    //   cornerFraction - how rounded the corners are (0 = sharp corners,
    //                    1 = a circle), as a fraction of half the size
    //   blurFraction   - how wide the soft edge is, same units
    private Texture2D MakeSoftSquareTexture(int size, float cornerFraction, float blurFraction)
    {
        Texture2D tex = new Texture2D(size, size);
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        float half   = size * 0.5f;
        float radius = half * cornerFraction;
        float blur   = half * blurFraction;

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
}
