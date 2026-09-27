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

    // Entries
    public static readonly Item IRON_ORE   = Add(0, "Iron Ore"  );
    public static readonly Item IRON_PLATE = Add(1, "Iron Plate");
}
