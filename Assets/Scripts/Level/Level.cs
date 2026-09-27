using System;

public class Level
{
    // Member Variables
    public          byte            Index           { get; private set; } 
    public          String          Name            { get; private set; }
    public readonly MapSize         mapSize          = new MapSize       ();
    public readonly MachineAmounts  machinesStarting = new MachineAmounts();
    public readonly ItemAmounts     deliveryStarting = new ItemAmounts   ();
    public readonly ItemAmounts     deliveryGoal     = new ItemAmounts   ();

    // Constructor
    public Level(byte index, String name)
    {
        Index = index;
        Name  = name;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Level", indentation);
        indentation++;

        DebugFile.Log("Index: " + Index, indentation);
        DebugFile.Log("Name: "  + Name,  indentation);

        mapSize         .Debug(indentation);
        machinesStarting.Debug(indentation);
        deliveryStarting.Debug(indentation);
        deliveryGoal    .Debug(indentation);
    }
}
