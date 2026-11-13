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

        // --------------------------------------------------
        // Level 0 - First Harvest (Tutorial)
        // --------------------------------------------------
        // Farm -> Mixer -> Delivery
        // Goal: Produce and deliver 6 Flour
        // Tutorial instructions will be added separately

        level = Add(entries, 180, "First Harvest");

        level.mapSize.Set(30, 15);

        level.machinesStarting
            .Modify(MachineCollection.PROCESSOR_FARM , 1)
            .Modify(MachineCollection.PROCESSOR_MIXER, 1)
            .Modify(MachineCollection.DELIVERY       , 1);

        level.deliveryGoal
            .Add(ItemCollection.FLOUR, 6);

        // --------------------------------------------------
        // Level 1 - Fresh Bread
        // --------------------------------------------------
        // Start by delivering Flour
        // Spend 8 Flour to purchase an Oven
        // Reconfigure the factory and deliver Bread

        level = Add(entries, 360, "Fresh Bread");

        level.mapSize.Set(45, 30);

        level.machinesStarting
            .Modify(MachineCollection.PROCESSOR_FARM , 1)
            .Modify(MachineCollection.PROCESSOR_MIXER, 1)
            .Modify(MachineCollection.DELIVERY       , 1);

        level.deliveryGoal
            .Add(ItemCollection.BREAD, 8);

        // --------------------------------------------------
        // Level 2 - Grand Bakery
        // --------------------------------------------------
        // Larger factory with multiple production chains
        // Requires Bread, Tarts, and Cakes
        // Can use the shop and logistics machines

        level = Add(entries, 1200, "Grand Bakery");

        level.mapSize.Set(90, 60);

        level.machinesStarting
            .Modify(MachineCollection.PROCESSOR_FARM , 2)
            .Modify(MachineCollection.PROCESSOR_MIXER, 2)
            .Modify(MachineCollection.PROCESSOR_OVEN , 1)
            .Modify(MachineCollection.DELIVERY       , 1);

        level.deliveryGoal
            .Add(ItemCollection.BREAD, 70)
            .Add(ItemCollection.TART , 35)
            .Add(ItemCollection.CAKE , 25);

        // --------------------------------------------------
        // Level 3 - Sandbox (Developer Level)
        // --------------------------------------------------
        // Give  10 of every valid machine
        // Give 200 of every resource
        // Easy goal for testing level completion

        level = Add(entries, 3600, "Sandbox");

        level.mapSize.Set(MapSize.VALUE_MAX, MapSize.VALUE_MAX);

        for (int i = 0; i < MachineCollection.Count(); i++)
        {
            byte index = (byte) i;
            level.machinesStarting.Modify(index, 10);
        }

        for (int i = 0; i < ItemCollection.Count(); i++)
        {
            Item item = ItemCollection.Get((byte)i);
            level.deliveryStarting.Add(item, 200);
        }

        level.deliveryGoal
            .Add(ItemCollection.BREAD, 205);

        // --------------------------------------------------
        // Level 4 - Impossible (Developer Level)
        // --------------------------------------------------
        // Five seconds to deliver 1000 Bread
        // Used to test timeout and failure

        level = Add(entries, 5, "Impossible");

        level.mapSize.Set(60, 30);

        level.deliveryGoal
            .Add(ItemCollection.BREAD, 1000);

        // Return Results
        return entries;
    }
    private static Level Add(List<Level> entries, int timeSec, String name)
    {
        byte index = (byte) entries.Count;
        Level level = new Level(index, timeSec, name);
        entries.Add(level);
        return level;
    }
}
