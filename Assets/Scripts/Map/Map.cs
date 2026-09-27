public class Map
{
    // Member Variables
    public readonly MapSize     mapSize     = new MapSize    ();
    public readonly MapView     mapView     = new MapView    ();
    public readonly MapMachines mapMachines = new MapMachines();

    // Constructor
    public Map(){}

    // Clear
    public void Clear()
    {
        mapSize    .Clear();
        mapView    .Clear();
        mapMachines.Clear();
    }

    // Game Tick
    public void GameTick()
    {
        mapView.GameTick();
    }

    // Start
    public void Start(MapSize size)
    {
        mapSize    .Set(size);
        mapView    .Set(size);
        mapMachines.Set(size);
    }

    // Link
    public void Link(GameLinks links)
    {
        mapMachines.Link(links);
    }

    // IO
    public void Save(IOWriter writer)
    {
        mapSize    .Save(writer);
        mapView    .Save(writer);
        mapMachines.Save(writer);
    }
    public void Load(IOReader reader)
    {
        mapSize    .Load(reader);
        mapView    .Load(reader);
        mapMachines.Load(reader);
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Map", indentation);
        indentation++;

        mapSize    .Debug(indentation);
        mapView    .Debug(indentation);
        mapMachines.Debug(indentation);
    }
}
