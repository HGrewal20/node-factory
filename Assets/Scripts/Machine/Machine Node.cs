using UnityEngine;

// Used for both input and output
// Both
//      Sets Offset once on creation
//      Sets ItemAllowed and Max once on creation (Except Processor)
// Processor
//      All nodes are made but disabled when no recipe is selected
//      When a recipe is selected, the appropriate nodes are turned on and ItemAllowed and Max are set
//      Input Max has just enough room for one recipe
// Processor Input
//      Amount is zeroed when recipe starts
//      Amount is incremented one at a time until max as it is transferred in
// Processor Output
//      Sets entire contents at once at machine output
//      Amount is decremented one at a time as it is transferred out
// Non Processors
//      Set Max to 1
//      Set Item Allowed to null (Any)
//      Set Is Enabled true always

public class MachineNode
{
    // Member Variables
    public bool         IsInput     { get; private set; }
    public bool         IsEnabled   { get; private set; } // "Hides" or "Deactivates" the node
    public int          Max         { get; private set; } // >= 1
    public int          Amount      { get; private set; } // >= 0 && <= Max
    public Vector2Int   Offset      { get; private set; } // Where it is connected on the machine
    public Item         ItemAllowed { get; private set; } // If null accepts anything
    public Item         ItemCurrent { get; private set; }
    private long        WireID;                           // Only used to load and link
    public  MachineWire Wire        { get; private set; }

    // Constructor
    public MachineNode(){}

    // Disconnect
    public void Disconnect()
    {
        if (Wire != null)
            MachineOperations.Disconnect(Wire);
    }

    // Transfer
    public void TryTransferInternal(MachineNode input)  // Transfer within machines (Flipper, Junction, Merger, and Splitter) from input to output
    {
        MachineNode output = this;

        if (  input .IsEmpty() )    return; // No Input    -> Fail
        if ( !output.IsEmpty() )    return; // Output Full -> Fail

        // Note: Doesn't check max or item allowed. Not required with current use case.

        // Put into output
        output.Amount      = input.Amount;
        output.ItemCurrent = input.ItemCurrent;

        // Clear input
        input.Amount      = 0;
        input.ItemCurrent = null;
    }
    public void TryTransferExternal(MachineNode output) // Input must not be null. Transfers via wire from output to input
    {
        MachineNode input = this;

        if ( output.IsEmpty() ) return; // No Output  -> Fail
        if ( input .IsFull () ) return; // Input Full -> Fail

        bool isAdded = input.TryAddInput(output.ItemCurrent);
        if (isAdded)
            output.OutputDecrement();
    }

    // Is Input
    public void SetInput    (bool isInput = true)   { IsInput = isInput; }
    public void SetOutput   ()                      { IsInput = false;   }

    // Is Enabled
    public void Enable       ()                 { SetEnabled(true      ); }
    public void Disable      ()                 { SetEnabled(false     ); }
    public void ToggleEnabled()                 { SetEnabled(!IsEnabled); }

    public void SetEnabled   (bool isEnabled)
    {
        IsEnabled = isEnabled;
        if ( !isEnabled)
        {
            Max         = 1;
            Amount      = 0;
            ItemAllowed = null;
            ItemCurrent = null;
        }
    }

    // Both
    public void SetOffset(Vector2Int offset)        { SetOffset(offset.x, offset.y); }
    public void SetOffset(int offsetX, int offsetY)
    {
        Offset = new Vector2Int(offsetX, offsetY);
    }

    public void SetAllowed(ItemAmount amount)      { SetAllowed(amount.Item, amount.Amount); }
    public void SetAllowed(Item item, int max)
    {
        if (max < 1)
            max = 1;

        ItemAllowed = item;
        ItemCurrent = null;
        Max         = max;
        Amount      = 0;
    }

    public void SetWire(MachineWire wire)
    {
        Wire = wire;
    }

    public MachineWireConnection GetWireConnection()
    {
        if (Wire == null)   return null;
        if (IsInput     )   return Wire.connectionOutput;
        else                return Wire.connectionInput;
    }
    public MachineNode GetNode()
    {
        MachineWireConnection wireConnection = GetWireConnection();
        if (wireConnection == null)     return null;
        else                            return wireConnection.GetNode();
    }

    public bool IsEmpty()
    {
        return (Amount == 0);
    }

    public bool IsFull()
    {
        return (Amount == Max);
    }

    // Input
    public void InputEmpty()
    {
        ItemCurrent = null;
        Amount      = 0;
    }

    private bool TryAddInput(Item item)
    {
        // If Disabled -> Fail
        if ( !IsEnabled )
            return false;

        // If Invalid Type -> Fail
        if (ItemAllowed != null && item != ItemAllowed)
            return false;

        // If Empty -> Add It
        if ( IsEmpty() )
        {
            ItemCurrent = item;
            Amount      = 1;
            return true;
        }

        // If Full -> Fail
        if ( IsFull() )
            return false;

        // Else -> Add it
        Amount++;
        return true;
    }

    // Output
    public void OutputDecrement()
    {
        Amount--;
        if (Amount <= 0)
        {
            Amount      = 0;
            ItemCurrent = null;
        }
    }

    public void OutputSet(ItemAmount amount)        { OutputSet(amount.Item, amount.Amount); }
    public void OutputSet(Item item, int amount)
    {
        ItemCurrent = item;
        Amount      = amount;
    }

    // Processor
    public void SetAccepts(ItemAmount amount)
    {
        Enable();
        Max         = amount.Amount;
        ItemAllowed = amount.Item;
    }

    // Link
    public void Link(GameLinks links)
    {
        Wire = links.GetWire(WireID);
    }

    // IO
    public void Save(IOWriter writer)
    {
        long wireID = (Wire == null) ? 0 : Wire.ID;

        writer.WriteBool      (IsInput    );
        writer.WriteBool      (IsEnabled  );
        writer.WriteInt       (Max        );
        writer.WriteInt       (Amount     );
        writer.WriteVector2Int(Offset     );
        writer.WriteItem      (ItemAllowed);
        writer.WriteItem      (ItemCurrent);
        writer.WriteLong      (wireID     );
    }
    public void Load(IOReader reader)
    {
        IsInput     = reader.ReadBool      ();
        IsEnabled   = reader.ReadBool      ();
        Max         = reader.ReadInt       ();
        Amount      = reader.ReadInt       ();
        Offset      = reader.ReadVector2Int();
        ItemAllowed = reader.ReadItem      ();
        ItemCurrent = reader.ReadItem      ();
        WireID      = reader.ReadLong      ();
    }

    // Debug
    public void Debug(int indentation)
    {
        long wireID = (Wire == null) ? 0 : Wire.ID;

        DebugFile.Log("MachineNode", indentation);
        indentation++;

        DebugFile.Log("IsInput:   " + IsInput  , indentation);
        DebugFile.Log("IsEnabled: " + IsEnabled, indentation);
        DebugFile.Log("Max:       " + Max      , indentation);
        DebugFile.Log("Amount:    " + Amount   , indentation);
        DebugFile.Log("Offset:    " + Offset   , indentation);
        DebugFile.Log("WireID:    " + WireID   , indentation);
        DebugFile.Log("wireID:    " + wireID   , indentation);

        if (ItemAllowed != null)   ItemAllowed.Debug(indentation);
        else                       DebugFile.Log("ItemAllowed: null", indentation);

        if (ItemCurrent != null)   ItemCurrent.Debug(indentation);
        else                       DebugFile.Log("ItemCurrent: null", indentation);
    }
}
