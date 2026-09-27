public class MachineFlipper : Machine
{
    // Constructor
    public MachineFlipper(byte machineTypeIndex, long id)
        : base(machineTypeIndex, id)
    {
        SetStandard( Height() );
    }

    // Game Tick
    public override void GameTick()
    {
        base.GameTick           ();
        this.TryTransferInternal();
    }
    private void TryTransferInternal()
    {
        int count = Count();
        if (count == 2)
        {
            TryTransferInternal(0, 1);
            TryTransferInternal(1, 0);
            return;
        }
        if (count == 3)
        {
            TryTransferInternal(0, 2);
            TryTransferInternal(1, 1);
            TryTransferInternal(2, 0);
            return;
        }
        if (count == 4)
        {
            TryTransferInternal(0, 3);
            TryTransferInternal(1, 2);
            TryTransferInternal(2, 1);
            TryTransferInternal(3, 0);
            return;
        }
    }

    // Getters
    public bool IsFlipper2() { return (MachineTypeIndex == MachineCollection.FLIPPER_2.Index); }
    public bool IsFlipper3() { return (MachineTypeIndex == MachineCollection.FLIPPER_3.Index); }
    public bool IsFlipper4() { return (MachineTypeIndex == MachineCollection.FLIPPER_4.Index); }

    private int Count()
    {
        return (Height() - 1) / 2;
    }

    private int Height()
    {
        if ( IsFlipper2() ) return 5;
        if ( IsFlipper3() ) return 7;
        else                return 9;
    }

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
        DebugFile.Log("MachineFlipper", indentation);
        indentation++;

        base.Debug(indentation);
    }
}
