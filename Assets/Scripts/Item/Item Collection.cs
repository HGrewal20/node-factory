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

    // Entries - Food Factory
    public static readonly Item WHEAT   = Add("Wheat"  );
    public static readonly Item BERRIES = Add("Berries");
    public static readonly Item MILK    = Add("Milk"   );

    public static readonly Item FLOUR   = Add("Flour"  );
    public static readonly Item JAM     = Add("Jam"    );
    public static readonly Item CREAM   = Add("Cream"  );

    public static readonly Item BREAD   = Add("Bread"  );
    public static readonly Item TART    = Add("Tart"   );
    public static readonly Item CAKE    = Add("Cake"   );
}
