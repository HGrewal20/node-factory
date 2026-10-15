using System;

public class Item
{
    // Member Variables
    public readonly byte   Index;
    public readonly String Name;

    // Constructor
    public Item(byte index, String name)
    {
        Index = index;
        Name  = name;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Item", indentation);
        indentation++;

        DebugFile.Log("Index: " + Index, indentation);
        DebugFile.Log("Name:  " + Name , indentation);
    }
}
