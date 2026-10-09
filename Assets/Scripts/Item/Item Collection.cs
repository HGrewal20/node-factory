using System;
using System.Collections.Generic;

public static class ItemCollection
{
    // Collection
    private static readonly List<Item> ENTRIES = new();
    private static Item Add(String name)
    {
        byte index = (byte) ENTRIES.Count;
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
    public static readonly Item SILICON = Add("Silicon"      );
    public static readonly Item WAFER   = Add("Wafer"        );
    public static readonly Item COPPER  = Add("Copper"       );
    public static readonly Item TRACE   = Add("Copper Trace" );
    public static readonly Item CHIP    = Add("Chip"         );
    public static readonly Item BOARD   = Add("Circuit Board");
}
