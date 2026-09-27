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

        level = new Level(0, "Test Level");
        level.mapSize.Set(1000, 1000);
        level.machinesStarting
            .Modify(MachineCollection.PROCESSOR_MINER  , 1)
            .Modify(MachineCollection.PROCESSOR_SMELTER, 1)
            .Modify(MachineCollection.DELIVERY         , 1);
        level.deliveryStarting
            .Add(ItemCollection.IRON_ORE  ,  10)
            .Add(ItemCollection.IRON_PLATE,   5);
        level.deliveryGoal
            .Add(ItemCollection.IRON_ORE  , 200)
            .Add(ItemCollection.IRON_PLATE, 500);
        entries.Add(level);

        return entries;
    }
}
