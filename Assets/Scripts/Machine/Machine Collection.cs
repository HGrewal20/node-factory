using System;
using System.Collections.Generic;

public static class MachineCollection
{
    // Collection
    private static readonly List<MachineType> ENTRIES = new();
    private static MachineType Add(String name)
    {
        byte index = (byte) ENTRIES.Count;
        MachineType entry = new MachineType(index, name);
        ENTRIES.Add(entry);
        return entry;
    }
    public static int Count()
    {
        return ENTRIES.Count;
    }
    public static MachineType Get(byte index)
    {
        return ENTRIES[index];
    }
    public static void Debug(int indentation)
    {
        DebugFile.Log("MachineCollection", indentation);
        indentation++;

        foreach (MachineType entry in ENTRIES)
            entry.Debug(indentation);
    }

    // Entries
    public static readonly MachineType DELIVERY             = Add("Delivery"         );     // 0
    public static readonly MachineType FLIPPER_2            = Add("Flipper 2 x 2"    );     // 1
    public static readonly MachineType FLIPPER_3            = Add("Flipper 3 x 3"    );     // 2
    public static readonly MachineType FLIPPER_4            = Add("Flipper 4 x 4"    );     // 3
    public static readonly MachineType JUNCTION_A           = Add("Junction A"       );     // 4
    public static readonly MachineType JUNCTION_B           = Add("Junction B"       );     // 5
    public static readonly MachineType MERGER_2             = Add("Merger 2 to 1"    );     // 6
    public static readonly MachineType MERGER_3             = Add("Merger 3 to 1"    );     // 7
    public static readonly MachineType MERGER_4             = Add("Merger 4 to 1"    );     // 8
    public static readonly MachineType SPLITTER_2           = Add("Splitter 1 to 2"  );     // 9
    public static readonly MachineType SPLITTER_3           = Add("Splitter 1 to 3"  );     // 10
    public static readonly MachineType SPLITTER_4           = Add("Splitter 1 to 4"  );     // 11
    public static readonly MachineType WIRE                 = Add("Wire"             );     // 12   Wire is not a real machine anymore. No shop entry. Not selectable. Treated as invalid.
    // --------------------------------------------------------------------------------     //      Wire Gives 1 million free wires to make it effectively free and infinite
    // --------------------------------------------------------------------------------     //      Above: Fixed (Doesn't Change)
    // --------------------------------------------------------------------------------     //      Below: Specific Recipes
    // Entries - Food Factory Processors
    public static readonly MachineType PROCESSOR_FARM  = Add("Farm" );                      // 13
    public static readonly MachineType PROCESSOR_MIXER = Add("Mixer");                      // 14
    public static readonly MachineType PROCESSOR_OVEN  = Add("Oven" );                      // 15
}
