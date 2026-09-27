public class MachineSplitter : Machine
{
    // Constructor
    public MachineSplitter(byte machineTypeIndex, long id)
        : base(machineTypeIndex, id)
    {
        SetStandard( Height() );
        inputs.OnlyKeepFirst();
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

                        TryTransferInternal(0, 0);
                        TryTransferInternal(0, 1);
        if (count >= 3) TryTransferInternal(0, 2);
        if (count >= 4) TryTransferInternal(0, 3);
    }

    // Getters
    public bool IsSplitter2() { return (MachineTypeIndex == MachineCollection.SPLITTER_2.Index); }
    public bool IsSplitter3() { return (MachineTypeIndex == MachineCollection.SPLITTER_3.Index); }
    public bool IsSplitter4() { return (MachineTypeIndex == MachineCollection.SPLITTER_4.Index); }

    private int Count()
    {
        return (Height() - 1) / 2;
    }

    private int Height()
    {
        if ( IsSplitter2() )    return 5;
        if ( IsSplitter3() )    return 7;
        else                    return 9;
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
        DebugFile.Log("MachineSplitter", indentation);
        indentation++;

        base.Debug(indentation);
    }
}
