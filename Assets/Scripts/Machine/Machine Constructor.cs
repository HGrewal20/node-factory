using UnityEngine;

public static class MachineConstructor
{
    // Create
    public static Machine Create(byte machineTypeIndex, long id)
    {
        if (machineTypeIndex == MachineCollection.DELIVERY  .Index)   return new MachineDelivery (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.FLIPPER_2 .Index)   return new MachineFlipper  (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.FLIPPER_3 .Index)   return new MachineFlipper  (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.FLIPPER_4 .Index)   return new MachineFlipper  (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.JUNCTION_A.Index)   return new MachineJunction (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.JUNCTION_B.Index)   return new MachineJunction (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.MERGER_2  .Index)   return new MachineMerger   (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.MERGER_3  .Index)   return new MachineMerger   (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.MERGER_4  .Index)   return new MachineMerger   (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.SPLITTER_2.Index)   return new MachineSplitter (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.SPLITTER_3.Index)   return new MachineSplitter (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.SPLITTER_4.Index)   return new MachineSplitter (machineTypeIndex, id);
        if (machineTypeIndex == MachineCollection.WIRE      .Index)   return new MachineWire     (machineTypeIndex, id);
        else                                                          return new MachineProcessor(machineTypeIndex, id);
    }

    public static Vector2Int GetSize(byte machineTypeIndex)
    {
        if (machineTypeIndex == MachineCollection.DELIVERY         .Index)   return new Vector2Int(3, 3);
        if (machineTypeIndex == MachineCollection.FLIPPER_2        .Index)   return new Vector2Int(5, 5);
        if (machineTypeIndex == MachineCollection.FLIPPER_3        .Index)   return new Vector2Int(5, 7);
        if (machineTypeIndex == MachineCollection.FLIPPER_4        .Index)   return new Vector2Int(5, 9);
        if (machineTypeIndex == MachineCollection.JUNCTION_A       .Index)   return new Vector2Int(3, 3);
        if (machineTypeIndex == MachineCollection.JUNCTION_B       .Index)   return new Vector2Int(3, 3);
        if (machineTypeIndex == MachineCollection.MERGER_2         .Index)   return new Vector2Int(5, 5);
        if (machineTypeIndex == MachineCollection.MERGER_3         .Index)   return new Vector2Int(5, 7);
        if (machineTypeIndex == MachineCollection.MERGER_4         .Index)   return new Vector2Int(5, 9);
        if (machineTypeIndex == MachineCollection.SPLITTER_2       .Index)   return new Vector2Int(5, 5);
        if (machineTypeIndex == MachineCollection.SPLITTER_3       .Index)   return new Vector2Int(5, 7);
        if (machineTypeIndex == MachineCollection.SPLITTER_4       .Index)   return new Vector2Int(5, 9);
        if (machineTypeIndex == MachineCollection.WIRE             .Index)   return new Vector2Int(1, 1);   // Invalid. Changes size.
        if (machineTypeIndex == MachineCollection.PROCESSOR_MINER  .Index)   return new Vector2Int(5, 3);
        if (machineTypeIndex == MachineCollection.PROCESSOR_SMELTER.Index)   return new Vector2Int(5, 3);
        else                                                                 return new Vector2Int(5, 1);   // Invalid
    }
}
