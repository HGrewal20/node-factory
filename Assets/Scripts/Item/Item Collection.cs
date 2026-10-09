using System;
using System.Collections.Generic;

public static class ItemCollection
{
    // Collection
    private static readonly List<Item> ENTRIES = new();
    private static Item Add(byte index, String name)
    {
        Item entry = new Item(index, name);
        ENTRIES.Add(entry);
        return entry;
    }
    public static int Count()
    {
        return ENTRIES.Count;
    }
    public static Item Get(byte index)
    {
        return ENTRIES[index];
    }
    public static void Debug(int indentation)
    {
        DebugFile.Log("ItemCollection", indentation);
        indentation++;

        foreach (Item entry in ENTRIES)
            entry.Debug(indentation);
    }

    // Entries - Circuit Theme (Michael)
    public static readonly Item SILICON = Add(0, "Silicon"      );
    public static readonly Item WAFER   = Add(1, "Wafer"        );
    public static readonly Item COPPER  = Add(2, "Copper"       );
    public static readonly Item TRACE   = Add(3, "Copper Trace" );
    public static readonly Item CHIP    = Add(4, "Chip"         );
    public static readonly Item BOARD   = Add(5, "Circuit Board");
}
