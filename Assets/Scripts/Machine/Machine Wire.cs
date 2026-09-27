// Should be either fully connected or fully disconnected
// Draw connection point -> entire wire connected at once
// Remove wire -> both points disconnected and wire removed
public class MachineWire : Machine
{
    // Constants
    public const byte DIRECTION_NONE  = 0;
    public const byte DIRECTION_RIGHT = 1;
    public const byte DIRECTION_UP    = 2;
    public const byte DIRECTION_DOWN  = 3;

    // Member Variables
    public          byte                  Direction         { get; private set; }
    public readonly MachineWireConnection connectionInput  = new MachineWireConnection(true );
    public readonly MachineWireConnection connectionOutput = new MachineWireConnection(false);

    // Constructor
    public MachineWire(byte machineTypeIndex, long id)
        : base(machineTypeIndex, id)
    {
        base.SetSize(1, 1);
    }

    // Disconnect
    public void DisconnectWire()
    {
        connectionInput .Disconnect();
        connectionOutput.Disconnect();
    }

    // Direction
    public void SetDirection(byte direction)
    {
        Direction = direction;
    }

    public bool IsDirectionRight(){ return (Direction == DIRECTION_RIGHT); }
    public bool IsDirectionUp   (){ return (Direction == DIRECTION_UP   ); }
    public bool IsDirectionDown (){ return (Direction == DIRECTION_DOWN ); }

    // Game Tick
    public override void GameTick()
    {
        base.GameTick           ();
        this.TryTransferExternal();
    }

    // Transfer
    public void TryTransferExternal()
    {
        MachineNode nodeInput  = connectionInput .GetNode();    if (nodeInput  == null) return;
        MachineNode nodeOutput = connectionOutput.GetNode();    if (nodeOutput == null) return;

        nodeInput.TryTransferExternal(nodeOutput);
    }

    // Link
    public override void Link(GameLinks links)
    {
        base            .Link(links);
        connectionInput .Link(links);
        connectionOutput.Link(links);
    }

    // IO
    public override void Save(IOWriter writer)
    {
        base            .Save(writer);
        writer          .WriteByte(Direction);
        connectionInput .Save(writer);
        connectionOutput.Save(writer);
    }
    public override void Load(IOReader reader)
    {
        base            .Load(reader);
        Direction       = reader.ReadByte();
        connectionInput .Load(reader);
        connectionOutput.Load(reader);
    }

    // Debug
    public override void Debug(int indentation)
    {
        DebugFile.Log("MachineWire", indentation);
        indentation++;

        base            .Debug(indentation);
        DebugFile.Log("Direction: " + Direction, indentation);
        connectionInput .Debug(indentation);
        connectionOutput.Debug(indentation);
    }
}
