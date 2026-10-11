using UnityEngine;

public class GameDisplay : MonoBehaviour
{
    private static readonly Color EMPTY_CELL_COLOR  = new Color(0.45f, 0.75f, 0.45f); // green
    private static readonly Color NODE_CONNECTED    = new Color(0.35f, 0.85f, 0.45f); // green
    private static readonly Color NODE_UNCONNECTED  = new Color(0.15f, 0.15f, 0.15f); // dark

    // A single white pixel, stretched and tinted into whatever rect/color we need. This is the only "texture" this script uses.
    private GameInteraction interaction;
    private GameHud         hud;

    void Awake()
    {
        interaction = GetComponent<GameInteraction>();
        hud         = GetComponent<GameHud        >();
    }

    void OnGUI()
    {
        GameData data = GameData.INSTANCE;
        if (!data.IsPlaying() || data.IsPaused || HudLevelCompleted.IsShowing)
            return;

        MapView   mapView  = data.map.mapView;
        RectInt   visible  = mapView.VisibleArea();
        Machine[] machines = data.map.mapMachines.GetArray(visible);

        DrawEmptyCells(mapView, visible, data.map.mapMachines);

        foreach (Machine machine in machines)
        {
            DrawMachine(mapView, machine);
            DrawNodes  (mapView, machine);
        }

        // if (hud != null)
        //     hud.DrawHud();
    }

    // Draws every empty cell in view as a solid square, inset by 1 pixel
    // on each side so you can see the grid lines between cells.
    private void DrawEmptyCells(MapView mapView, RectInt visible, MapMachines mapMachines)
    {
        GUI.color = EMPTY_CELL_COLOR;

        for (int x = visible.xMin; x < visible.xMax; x++)
        {
            for (int y = visible.yMin; y < visible.yMax; y++)
            {
                if ( mapMachines.Get(x, y) != null )
                    continue; // a machine already draws this cell

                Rect screenRect = CellToScreen(mapView, x, y, 1);

                screenRect.x      += 1;
                screenRect.y      += 1;
                screenRect.width  -= 2;
                screenRect.height -= 2;

                if (screenRect.width <= 0 || screenRect.height <= 0)
                    continue; // zoomed out too far for this to show up

                GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
            }
        }
    }

    // Whole machine footprint, one flat solid color.
    private void DrawMachine(MapView mapView, Machine machine)
    {
        RectInt gridRect = machine.Rect;
        Rect screenRect = CellToScreen(mapView, gridRect.xMin, gridRect.yMin, gridRect.width, gridRect.height);

        GUI.color = ColorFor(machine);
        GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
    }

    // Draws a small marker on every enabled node so you can see exactly
    // where to click to connect it. Dark = not connected. Green = connected.
    // Smaller than the cell, so the gap around it reads as a border.
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

            float insetX = screenRect.width  * 0.28f;
            float insetY = screenRect.height * 0.28f;
            screenRect.x      += insetX;
            screenRect.y      += insetY;
            screenRect.width  -= insetX * 2f;
            screenRect.height -= insetY * 2f;

            GUI.color = node.IsConnected() ? NODE_CONNECTED : NODE_UNCONNECTED;
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
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
}
