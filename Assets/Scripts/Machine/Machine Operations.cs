// Use this class to make changes to the machines
// Wire Connections:
//      Machine -> Machine Nodes (inputs & outputs) -> Machine Node
//      To
//      Machine Wire -> Machine Wire Connection (input & output)
using UnityEngine;

public static class MachineOperations
{
    // Try Connect
    public static bool TryConnect(
        Machine  inputMachine, int  inputNodeIndex,     // Wire Input  = Machine Output
        Machine outputMachine, int outputNodeIndex)     // Wire Output = Machine Input
    {
        // If No Free Wire Available -> Fail
        GameData data = GameData.INSTANCE;
        if (data.machinesAvailable.AmountWire() < 1)
            return false;

        // Get Nodes to Connect
        MachineNode inputNode  = inputMachine .outputs.Get( inputNodeIndex);
        MachineNode outputNode = outputMachine.inputs .Get(outputNodeIndex);

        // Check Position
        Vector2Int inputPos  = (inputMachine .Pos + inputNode .Offset);
        Vector2Int outputPos = (outputMachine.Pos + outputNode.Offset);

        Vector2Int start     = Vector2Int.zero;
        Vector2Int end       = Vector2Int.zero;
        bool       isGood    = false;
        byte       direction = MachineWire.DIRECTION_NONE;

        // Try Horizontal Connection
        if (inputPos.y == outputPos.y)
        {
            int x0 =  inputPos.x;
            int x1 = outputPos.x;

            int width = (x1 - x0);
            if (width < 2)
                return false;

            int y     = inputPos.y;
            start     = new Vector2Int(x0 + 1, y); // Start one square right of input  end
            end       = new Vector2Int(x1 - 1, y); // End   one square left  of output end
            isGood    = true;
            direction = MachineWire.DIRECTION_RIGHT;
        }
        // Try Vertical Connection
        if (inputPos.x == outputPos.x)
        {
            if (inputPos.y < outputPos.y)   direction = MachineWire.DIRECTION_UP;
            else                            direction = MachineWire.DIRECTION_DOWN;

            int y0 = Mathf.Min(inputPos.y, outputPos.y);
            int y1 = Mathf.Max(inputPos.y, outputPos.y);

            int height = (y1 - y0);
            if (height < 1)
                return false;

            int x  = inputPos.x;
            start  = new Vector2Int(x, y0 + 1);  // Start one square above lower  end
            end    = new Vector2Int(x, y1 - 1);  // Start one square below higher end;
            isGood = true;
        }

        // If Orientation Invalid -> Fail
        if ( !isGood )
            return false;

        // Determine Size
        Vector2Int size = new Vector2Int(
            end.x - start.x + 1,
            end.y - start.y + 1
        );

        // Try Add Machine
        Machine machine = TryPlaceMachine( MachineCollection.WIRE.Index, start, size );
        if (machine == null)
            return false;

        // Set Wire Direction
        MachineWire wire = (MachineWire) machine;
        wire.SetDirection(direction);
        
        // Worked
        return true;
    }

    // Disconnect
    public static void Disconnect(MachineWire wire)
    {
        RemoveMachine(wire);
    }

    // Remove Machine
    public static void RemoveMachine(Machine machine)
    {
        MachineWire wire = machine as MachineWire;
        if (wire != null)
        {
            wire.DisconnectWire();                                      // Break Corresponding Connections
        }

        machine.Disconnect();                                           // Break old connections

        GameData data = GameData.INSTANCE;
        data.machinesUsed     .Remove(machine                    );     // Remove from list
        data.map.mapMachines  .Remove(machine                    );     // Remove from map
        data.machinesAvailable.Modify(machine.MachineTypeIndex, 1);     // Add to available parts
    }

    // Try Place Machine
    public static Machine TryPlaceMachine(byte machineTypeIndex, Vector2Int pos)                            { return TryPlaceMachine( machineTypeIndex, pos, new Vector2Int(1, 1) ); }
    public static Machine TryPlaceMachine(byte machineTypeIndex, Vector2Int pos, Vector2Int sizeOverwrite)
    {
        GameData data = GameData.INSTANCE;

        Vector2Int size = MachineConstructor.GetSize(machineTypeIndex);

        // If No Part Available -> Fail
        if (data.machinesAvailable.Amount(machineTypeIndex) < 1)
            return null;

        // If No Space In Map -> Fail
        RectInt rect = new RectInt(pos, size);

        bool isFree = data.map.mapMachines.IsFree(rect);
        if ( !isFree )
            return null;

        // Consume Part
        data.machinesAvailable.Modify(machineTypeIndex, -1);

        // Create Machine
        Machine machine = data.machinesUsed.Add(machineTypeIndex);  // Also adds to list
        if (sizeOverwrite.x != 1 || sizeOverwrite.y != 1)
        machine.SetSize(sizeOverwrite);
        machine.SetPos (pos          );

        // Add to Map
        data.map.mapMachines.Add(machine);

        // Return result
        return machine;
    }
}
