using UnityEngine;

public class Machine
{
    // Member Variables
    public          byte           MachineTypeIndex { get; private set; }
    public          long           ID               { get; private set; }
    public          Vector2Int     Pos              { get; private set; }
    public          Vector2Int     Size             { get; private set; }
    public          RectInt        Rect             { get; private set; }
    public readonly MachineNodes   inputs           = new MachineNodes();
    public readonly MachineNodes   outputs          = new MachineNodes();

    // Constructor
    public Machine(byte machineTypeIndex, long id)
    {
        MachineTypeIndex = machineTypeIndex;
        ID               = id;
    }

    // Game Tick
    public virtual void GameTick(){}

    // Disconnect
    public void Disconnect()
    {
        inputs .Disconnect();
        outputs.Disconnect();
    }

    // Set
    public void SetPos(Vector2Int pos)
    {
        Pos = pos;
        ComputeRect();
    }

    public void SetSize(Vector2Int size)        { SetSize(size.x, size.y); }
    public void SetSize(int width, int height)
    {
        Size = new Vector2Int(width, height);
        ComputeRect();
    }

    public void SetStandard(int height) // Used by: Flipper, Merger, Processor, Splitter
    {
        int count  = (height - 1) / 2;

        this   .SetSize        (5, height);
        inputs .AddInputsLeft  (true, count,    1, null);
        outputs.AddOutputsRight(true, count, 4, 1, null);
    }

    private void ComputeRect()
    {
        Rect = new RectInt(Pos, Size);
    }

    // Processor
    public void DisableAllNodes()
    {
        inputs .DisableAll();
        outputs.DisableAll();
    }

    // Transfer
    public void TryTransferInternal(int indexInput, int indexOutput)    // Used by: flipper, junction, merger, and splitter
    {
        MachineNode nodeInput  = inputs .Get(indexInput );
        MachineNode nodeOutput = outputs.Get(indexOutput);

        nodeOutput.TryTransferInternal(nodeInput);
    }

    // Link
    public virtual void Link(GameLinks links)
    {
        inputs .Link(links);
        outputs.Link(links);
    }

    // IO
    public virtual void Save(IOWriter writer)
    {
        writer.WriteByte      (MachineTypeIndex);
        writer.WriteLong      (ID              );
        writer.WriteVector2Int(Pos             );
        writer.WriteVector2Int(Size            );
        writer.WriteRectInt   (Rect            );

        inputs .Save(writer);
        outputs.Save(writer);
    }
    public virtual void Load(IOReader reader)
    {
        // reader.ReadByte();   // Note: Done Elsewhere
        // reader.ReadLong();   // Note: Done Elsewhere
        Pos  = reader.ReadVector2Int();
        Size = reader.ReadVector2Int();
        Rect = reader.ReadRectInt   ();

        inputs .Load(reader);
        outputs.Load(reader);
    }

    // Debug
    public virtual void Debug(int indentation)
    {
        DebugFile.Log("Machine", indentation);
        indentation++;

        DebugFile.Log("MachineTypeIndex = " + MachineTypeIndex, indentation);
        DebugFile.Log("ID               = " + ID              , indentation);
        DebugFile.Log("Pos              = " + Pos             , indentation);
        DebugFile.Log("Size             = " + Size            , indentation);
        DebugFile.Log("Rect             = " + Rect            , indentation);

        inputs .Debug(indentation);
        outputs.Debug(indentation);
    }
}
