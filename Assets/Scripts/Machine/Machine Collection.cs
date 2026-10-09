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
    public static readonly MachineType DELIVERY             = Add("Delivery"         );
    public static readonly MachineType FLIPPER_2            = Add("Flipper 2x2"      );
    public static readonly MachineType FLIPPER_3            = Add("Flipper 3x3"      );
    public static readonly MachineType FLIPPER_4            = Add("Flipper 4x4"      );
    public static readonly MachineType JUNCTION_A           = Add("Junction A"       );
    public static readonly MachineType JUNCTION_B           = Add("Junction B"       );
    public static readonly MachineType MERGER_2             = Add("Merger 2 to 1"    );
    public static readonly MachineType MERGER_3             = Add("Merger 3 to 1"    );
    public static readonly MachineType MERGER_4             = Add("Merger 4 to 1"    );
    public static readonly MachineType SPLITTER_2           = Add("Splitter 1 to 2"  );
    public static readonly MachineType SPLITTER_3           = Add("Splitter 1 to 3"  );
    public static readonly MachineType SPLITTER_4           = Add("Splitter 1 to 4"  );
    public static readonly MachineType WIRE                 = Add("Wire"             ); // Here & Above: Fixed (Doesn't Change)
    public static readonly MachineType PROCESSOR_MINER      = Add("Miner"            ); // Here & Below: Add More Processors Later
    public static readonly MachineType PROCESSOR_SMELTER    = Add("Smelter"          ); // Here & Below: Add More Processors Later
}
