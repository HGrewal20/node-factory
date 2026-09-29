using UnityEngine;

// Draws whatever is currently placed on the map, as colored boxes.
// This is Step 1 of the UI: just drawing. Clicking/placing comes in Step 2.
public class GameDisplay : MonoBehaviour
{
    // A single white pixel we can tint any color and stretch into a rectangle.
    private Texture2D pixel;

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
        GameData data = GameData.INSTANCE;
        data.LevelStart( LevelCollection.Get(0) );

        // TEMP: place a few machines by hand so there is something to see.
        // Step 2 replaces this block with real click-to-place.
        MachineOperations.TryPlaceMachine(MachineCollection.PROCESSOR_MINER  .Index, new Vector2Int(500, 500));
        MachineOperations.TryPlaceMachine(MachineCollection.PROCESSOR_SMELTER.Index, new Vector2Int(508, 500));
        MachineOperations.TryPlaceMachine(MachineCollection.DELIVERY         .Index, new Vector2Int(516, 500));

        pixel = new Texture2D(1, 1);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();
    }

    void OnGUI()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() )
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

        foreach (Machine machine in machines)
            DrawMachine(mapView, machine);
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

    private Color ColorFor(Machine machine)
    {
        if (machine is MachineWire)       return new Color(0.55f, 0.55f, 0.55f); // gray
        if (machine is MachineDelivery)   return new Color(0.90f, 0.70f, 0.20f); // gold
        if (machine is MachineProcessor)  return new Color(0.30f, 0.60f, 0.90f); // blue

        return Color.white; // flippers, junctions, mergers, splitters - not placed yet
    }
}
