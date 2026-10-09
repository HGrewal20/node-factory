using System.Collections.Generic;

public static class LevelCollection
{
    // Collection
    private static readonly List<Level> ENTRIES = CreateLevels();
    public  static int Count()
    {
        return ENTRIES.Count;
    }
    public  static Level Get(byte index)
    {
        return ENTRIES[index];
    }
    public static void Debug(int indentation)
    {
        DebugFile.Log("LevelCollection", indentation);
        indentation++;
        
        foreach (Level entry in ENTRIES)
            entry.Debug(indentation);
    }

    // Create Levels
    private static List<Level> CreateLevels()
    {
        List<Level> entries = new();
        Level level = null;

        // Level 0 - First Factory. Short on purpose for the video: (Michael)
        // place Miner -> Smelter -> Delivery, connect them, wait about 20 seconds.  (Michael)    
        level = new Level(0, "First Factory");
        level.mapSize.Set(MapSize.VALUE_MIN, MapSize.VALUE_MIN);
        level.machinesStarting
            .Modify(MachineCollection.WIRE             , 4)
            .Modify(MachineCollection.PROCESSOR_MINER  , 1)
            .Modify(MachineCollection.PROCESSOR_SMELTER, 1)
            .Modify(MachineCollection.DELIVERY         , 1);
        level.deliveryGoal
            .Add(ItemCollection.WAFER     ,  10);
        entries.Add(level);

        // Level 1 - Test Level (kept for testing)
        level = new Level(1, "Test Level");
        level.mapSize.Set(MapSize.VALUE_MAX, MapSize.VALUE_MAX);
        level.machinesStarting
            .Modify(MachineCollection.WIRE             , 2)
            .Modify(MachineCollection.PROCESSOR_MINER  , 1)
            .Modify(MachineCollection.PROCESSOR_SMELTER, 1)
            .Modify(MachineCollection.DELIVERY         , 1);
        level.deliveryStarting
            .Add(ItemCollection.SILICON   ,  10)
            .Add(ItemCollection.WAFER     ,   5);
        level.deliveryGoal
            .Add(ItemCollection.SILICON   , 200)
            .Add(ItemCollection.WAFER     , 500);
        entries.Add(level);

        return entries;
    }
}
