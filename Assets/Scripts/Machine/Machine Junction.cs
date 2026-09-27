public class MachineJunction : Machine
{
    // Constructor
    public MachineJunction(byte machineTypeIndex, long id)
        : base(machineTypeIndex, id)
    {
        base.SetSize(3, 3);

        inputs .AddInputsLeft  (true, 1,    1, null);   // Left  = input
        outputs.AddOutputsRight(true, 1, 2, 1, null);   // Right = output

        bool isJunctionA = IsJunctionA();
        if (isJunctionA)
        {
        inputs .Add( true, true, 1, 0, 1, null);         // Top?  = input
        outputs.Add(false, true, 1, 2, 1, null);         // Bot?  = output
        }
        else // if (isJunctionB)
        {
        outputs.Add(false, true, 1, 0, 1, null);          // Top?  = output
        inputs .Add( true, true, 1, 2, 1, null);          // Bot?  = input
        }
    }

    // Game Tick
    public override void GameTick()
    {
        base.GameTick           ();
        this.TryTransferInternal();
    }
    private void TryTransferInternal()
    {
        TryTransferInternal(0, 0);
        TryTransferInternal(0, 1);
        TryTransferInternal(1, 0);
        TryTransferInternal(1, 1);
    }

    // Getters
    public bool IsJunctionA() { return (MachineTypeIndex == MachineCollection.JUNCTION_A.Index); }
    public bool IsJunctionB() { return (MachineTypeIndex == MachineCollection.JUNCTION_B.Index); }

    // IO
    public override void Save(IOWriter writer)
    {
        base.Save(writer);
    }
    public override void Load(IOReader reader)
    {
        base.Load(reader);
    }

    // Debug
    public override void Debug(int indentation)
    {
        DebugFile.Log("MachineJunction", indentation);
        indentation++;

        base.Debug(indentation);
    }
}
