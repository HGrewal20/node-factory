using System.Collections.Generic;

public static class ShopCollection
{
    // Collection
    private static readonly List<ShopEntry> ENTRIES = CreateEntries();
    public  static int Count()
    {
        return ENTRIES.Count;
    }
    public static ShopEntry Get(byte index)
    {
        return ENTRIES[index];
    }
    // Debug
    public static void Debug(int indentation)
    {
        DebugFile.Log("ShopCollection", indentation);
        indentation++;

        foreach (ShopEntry entry in ENTRIES)
            entry.Debug(indentation);
    }

    // Create Entries
    private static ShopEntry AddEntry(List<ShopEntry> entries, MachineType machineType)
    {
        ShopEntry entry = new ShopEntry(MachineCollection.PROCESSOR_SMELTER);
        entries.Add(entry);
        return entry;
    }
    private static List<ShopEntry> CreateEntries()
    {
        List<ShopEntry> entries = new();
        AddEntry(entries, MachineCollection.DELIVERY         ).cost.Add(ItemCollection.IRON_PLATE, 20);     // Delivery
        AddEntry(entries, MachineCollection.FLIPPER_2        ).cost.Add(ItemCollection.IRON_PLATE,  5);     // Flippers
        AddEntry(entries, MachineCollection.FLIPPER_3        ).cost.Add(ItemCollection.IRON_PLATE,  8);
        AddEntry(entries, MachineCollection.FLIPPER_2        ).cost.Add(ItemCollection.IRON_PLATE, 10);
        AddEntry(entries, MachineCollection.JUNCTION_A       ).cost.Add(ItemCollection.IRON_PLATE,  2);     // Junctions
        AddEntry(entries, MachineCollection.JUNCTION_B       ).cost.Add(ItemCollection.IRON_PLATE,  2);
        AddEntry(entries, MachineCollection.MERGER_2         ).cost.Add(ItemCollection.IRON_PLATE,  5);     // Mergers
        AddEntry(entries, MachineCollection.MERGER_3         ).cost.Add(ItemCollection.IRON_PLATE,  8);
        AddEntry(entries, MachineCollection.MERGER_4         ).cost.Add(ItemCollection.IRON_PLATE, 10);
        AddEntry(entries, MachineCollection.SPLITTER_2       ).cost.Add(ItemCollection.IRON_PLATE,  5);     // Splitters
        AddEntry(entries, MachineCollection.SPLITTER_3       ).cost.Add(ItemCollection.IRON_PLATE,  8);
        AddEntry(entries, MachineCollection.SPLITTER_4       ).cost.Add(ItemCollection.IRON_PLATE, 10);
        AddEntry(entries, MachineCollection.WIRE             ).cost.Add(ItemCollection.IRON_PLATE,  1);     // Wire
        AddEntry(entries, MachineCollection.PROCESSOR_MINER  ).cost.Add(ItemCollection.IRON_PLATE,  8);     // Processors
        AddEntry(entries, MachineCollection.PROCESSOR_SMELTER).cost.Add(ItemCollection.IRON_PLATE, 12).Add(ItemCollection.IRON_ORE, 4);
        return entries;
    }
}
