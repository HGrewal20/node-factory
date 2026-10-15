using System;
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
        Level       level   = null;

        // Level 0 - First Factory. Short on purpose for the video: (Michael)
        // place Miner -> Smelter -> Delivery, connect them, wait about 20 seconds.  (Michael)    
        level = Add(entries, "First Factory");
        level.mapSize.Set(MapSize.VALUE_MIN * 2, MapSize.VALUE_MIN);
        level.machinesStarting                                      // No Need to add wires, adding a few won't do anything as they are already given 1 million
            .Modify(MachineCollection.PROCESSOR_MINER  , 1)
            .Modify(MachineCollection.PROCESSOR_SMELTER, 1)
            .Modify(MachineCollection.DELIVERY         , 1);
        level.deliveryGoal
            .Add(ItemCollection.WAFER     ,  10);

        // Level 1 - Large Level
        level = Add(entries, "Large Level");
        level.mapSize.Set(MapSize.VALUE_MAX, MapSize.VALUE_MAX);
        level.machinesStarting
            .Modify(MachineCollection.PROCESSOR_MINER  , 1)
            .Modify(MachineCollection.PROCESSOR_SMELTER, 1)
            .Modify(MachineCollection.DELIVERY         , 1);
        level.deliveryStarting
            .Add(ItemCollection.SILICON   ,  10)
            .Add(ItemCollection.WAFER     ,   5);
        level.deliveryGoal
            .Add(ItemCollection.SILICON   , 200)
            .Add(ItemCollection.WAFER     , 500);

        // Level 2 - Test Level
        level = Add(entries, "Test");
        level.mapSize.Set(MapSize.VALUE_MIN * 4, MapSize.VALUE_MIN * 2);
        for (int i = 0; i < MachineCollection.Count(); i++)
        {
        level.machinesStarting.Modify( (byte) i,  10);        // Give 10 of every machine
        }
        for (int i = 0; i < ItemCollection.Count(); i++)
        {
        Item item = ItemCollection.Get( (byte) i );
        level.deliveryStarting.Add   (item, 200);             // Give 200 of every resource
        }
        level.deliveryGoal.Add(ItemCollection.WAFER,  205);   // Quickly Achivable goal

        return entries;
    }
    private static Level Add(List<Level> entries, String name)
    {
        byte index = (byte) entries.Count;
        Level level = new Level(index, name);
        entries.Add(level);
        return level;
    }
}
