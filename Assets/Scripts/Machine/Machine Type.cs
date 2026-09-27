using System;

public class MachineType
{
    // Member Variables
    public byte   Index { get; private set; }
    public String Name  { get; private set; }

    // Constructor
    public MachineType(byte index, String name)
    {
        Index = index;
        Name  = name;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MachineType", indentation);
        indentation++;

        DebugFile.Log("Index: " + Index, indentation);
        DebugFile.Log("Name: "  + Name,  indentation);
    }
}
