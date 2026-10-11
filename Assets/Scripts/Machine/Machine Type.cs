using System;

public class MachineType
{
    // Member Variables
    public readonly  byte   Index ;
    public readonly  String Name;

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
