using System;

public class Level
{
    // Member Variables
    public readonly byte            Index;
    public readonly int             TimeSec;
    public readonly String          Name;
    public readonly MapSize         mapSize          = new MapSize       ();
    public readonly MachineAmounts  machinesStarting = new MachineAmounts();
    public readonly ItemAmounts     deliveryStarting = new ItemAmounts   ();
    public readonly ItemAmounts     deliveryGoal     = new ItemAmounts   ();

    // Constructor
    public Level(byte index, int timeSec, String name)
    {
        Index   = index;
        TimeSec = timeSec;
        Name    = name;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Level", indentation);
        indentation++;

        DebugFile.Log("Index:   " + Index  , indentation);
        DebugFile.Log("TimeSec: " + TimeSec, indentation);
        DebugFile.Log("Name:    " + Name   , indentation);

        mapSize         .Debug(indentation);
        machinesStarting.Debug(indentation);
        deliveryStarting.Debug(indentation);
        deliveryGoal    .Debug(indentation);
    }
}
