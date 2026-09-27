using System;

public class MachineAmounts
{
    // Member Variables
    private readonly int[] amounts = new int[ MachineCollection.Count() ];  // Cannot be negative

    // Constructor
    public MachineAmounts(){}

    // Clear
    public void Clear()
    {
         Array.Clear(amounts, 0, amounts.Length);
    }

    // Get
    public int Amount(byte index)
    {
        return amounts[index];
    }
    public int AmountWire()
    {
        return Amount( MachineCollection.WIRE.Index );
    }

    // Set
    public void Set(MachineAmounts other)
    {
        for (int i = 0; i < MachineCollection.Count(); i++)
            amounts[i] = other.amounts[i];
    }

    // Modify
    public MachineAmounts Modify(MachineType machineType, int delta) { return Modify(machineType.Index, delta); }
    public MachineAmounts Modify(byte        index      , int delta)
    {
        int amount = amounts[index] + delta;
        if (amount >= 0)
            amounts[index] = amount;
            
        return this;
    }

    // IO
    public void Save(IOWriter writer)
    {
        foreach (int amount in amounts)
            writer.WriteInt(amount);
    }
    public void Load(IOReader reader)
    {
        int count = amounts.Length;
        for (int i = 0; i < count; i++)
            amounts[i] = reader.ReadInt();
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MachineAmounts", indentation);
        indentation++;

        for (byte i = 0; i < amounts.Length; i++)
            DebugFile.Log("Machine: " + i + ", Amount: " + amounts[i], indentation);
    }
}
