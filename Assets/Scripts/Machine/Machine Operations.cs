// Use this class to make changes to the machines
using UnityEngine;

public static class MachineOperations
{
    // Try Connect
    public static bool TryConnect(Machine oneMachine, MachineNode oneNode)
    {
        // Fail if Not Contained
        if ( !oneMachine.Contains(oneNode) )
            return false;

        // Fail if can't connect
        if ( !oneNode.CanConnect() )
            return false;

        // Get Direction
        int dx = 0;
        int dy = 0;

        switch (oneNode.Side)
        {
            case MachineNode.SIDE_NONE:             return false;
            case MachineNode.SIDE_TOP:   dy = +1;   break;
            case MachineNode.SIDE_LEFT:  dx = -1;   break;
            case MachineNode.SIDE_RIGHT: dx = +1;   break;
            case MachineNode.SIDE_BOT:   dy = -1;   break;
        }

        // Find Next Machine
        Vector2Int pos = (oneMachine.Pos + oneNode.Offset);
        
        Machine twoMachine = GameData.INSTANCE.map.mapMachines.FindNext(pos, dx, dy, out Vector2Int machinePosition);
        if (twoMachine == null)
            return false;

        // Find Node
        MachineNode twoNode = twoMachine.GetNode(machinePosition);
        if (twoNode == null)
            return false;

        // Fail if can't connect
        if ( !oneNode.CanConnect() )
            return false;

        // Fail If Invalid Types -> Need one input and one output
        if (oneNode.IsInput == twoNode.IsInput)
            return false;

        // If No Free Wire Available -> Fail
        GameData data = GameData.INSTANCE;
        if (data.machinesAvailable.AmountWire() < 1)
            return false;

        // Normalize connection:
        //
        // inputMachine  / inputNode  = machine OUTPUT
        // outputMachine / outputNode = machine INPUT
        //
        Machine     inputMachine;
        MachineNode inputNode;

        Machine     outputMachine;
        MachineNode outputNode;

        if (!oneNode.IsInput)
        {
            inputMachine  = oneMachine;
            inputNode     = oneNode;

            outputMachine = twoMachine;
            outputNode    = twoNode;
        }
        else
        {
            inputMachine  = twoMachine;
            inputNode     = twoNode;

            outputMachine = oneMachine;
            outputNode    = oneNode;
        }

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
        else if (inputPos.x == outputPos.x)
        {
            if (inputPos.y < outputPos.y)   direction = MachineWire.DIRECTION_UP;
            else                            direction = MachineWire.DIRECTION_DOWN;

            int y0 = Mathf.Min(inputPos.y, outputPos.y);
            int y1 = Mathf.Max(inputPos.y, outputPos.y);

            int height = (y1 - y0);
            if (height < 2)
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

        // Link
        int  inputIndex =  inputMachine.outputs.IndexOf( inputNode);
        int outputIndex = outputMachine.inputs .IndexOf(outputNode);

        Debug.Assert(inputIndex  >= 0);     // Should never happen. Safety Check.
        Debug.Assert(outputIndex >= 0);

        bool isConnectedInput  = wire.connectionInput .TryConnect( inputIndex,  inputMachine);
        bool isConnectedOutput = wire.connectionOutput.TryConnect(outputIndex, outputMachine);

        Debug.Assert(isConnectedInput , "MachineOperations.TryConnect - Failed to Connect Input" ); // Should never happen. Safety Check.
        Debug.Assert(isConnectedOutput, "MachineOperations.TryConnect - Failed to Connect Output"); // Already checked to be free so should be fine.

         inputNode.SetWire(wire);
        outputNode.SetWire(wire);
        
        // Worked
        return true;
    }
    public static int TryConnectAll(Machine machine)
    {
        return  TryConnectAll(machine, machine.inputs ) + 
                TryConnectAll(machine, machine.outputs); 
    }
    private static int TryConnectAll(Machine machine, MachineNodes nodes)
    {
        int connections = 0;

        int count = nodes.Count();
        for (int i = 0; i < count; i++)
        {
            MachineNode node = nodes.Get(i);
            if ( TryConnect(machine, node) )
                connections++;
        }

        return connections;
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
