using System.Collections.Generic;

public static class ShopCollection
{
    // Constants
    public const int MAX_SHOP_COST_ENTRIES = 4;

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
        ShopEntry entry = new ShopEntry(machineType);
        entries.Add(entry);
        return entry;
    }
    private static List<ShopEntry> CreateEntries()
    {
        List<ShopEntry> entries = new();

        // Delivery
        AddEntry(entries, MachineCollection.DELIVERY).cost.Add(ItemCollection.BREAD, 10);

        // Flippers
        AddEntry(entries, MachineCollection.FLIPPER_2).cost.Add(ItemCollection.FLOUR, 2);
        AddEntry(entries, MachineCollection.FLIPPER_3).cost.Add(ItemCollection.FLOUR, 3);
        AddEntry(entries, MachineCollection.FLIPPER_4).cost.Add(ItemCollection.FLOUR, 4);

        // Junctions
        AddEntry(entries, MachineCollection.JUNCTION_A).cost.Add(ItemCollection.FLOUR, 1);
        AddEntry(entries, MachineCollection.JUNCTION_B).cost.Add(ItemCollection.FLOUR, 1);

        // Mergers
        AddEntry(entries, MachineCollection.MERGER_2).cost.Add(ItemCollection.FLOUR, 2);
        AddEntry(entries, MachineCollection.MERGER_3).cost.Add(ItemCollection.FLOUR, 3);
        AddEntry(entries, MachineCollection.MERGER_4).cost.Add(ItemCollection.FLOUR, 4);

        // Splitters
        AddEntry(entries, MachineCollection.SPLITTER_2).cost.Add(ItemCollection.FLOUR, 2);
        AddEntry(entries, MachineCollection.SPLITTER_3).cost.Add(ItemCollection.FLOUR, 3);
        AddEntry(entries, MachineCollection.SPLITTER_4).cost.Add(ItemCollection.FLOUR, 4);

        // Wire is invalid. No shop entry.

        // Food Factory Processors
        AddEntry(entries, MachineCollection.PROCESSOR_FARM ).cost.Add(ItemCollection.BREAD, 6);
        AddEntry(entries, MachineCollection.PROCESSOR_MIXER).cost.Add(ItemCollection.FLOUR, 4);
        AddEntry(entries, MachineCollection.PROCESSOR_OVEN ).cost.Add(ItemCollection.FLOUR, 8);

        // Safety Check
        CheckEntries(entries);

        // Return Result
        return entries;
    }
    private static void CheckEntries(List<ShopEntry> entries)
    {
        foreach (ShopEntry entry in entries)
        {
            ItemAmounts cost = entry.cost;

            if (cost.Count() > MAX_SHOP_COST_ENTRIES)
            {
                UnityEngine.Debug.Log("Error! Shop Entry has too many cost entries. Max allowed is 4. Removing Extras.");
                cost.Limit(MAX_SHOP_COST_ENTRIES);
            }
        }
    }
}
