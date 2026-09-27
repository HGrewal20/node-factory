public class MachineMerger : Machine
{
    // Constructor
    public MachineMerger(byte machineTypeIndex, long id)
        : base(machineTypeIndex, id)
    {
        SetStandard( Height() );
        outputs.OnlyKeepFirst();
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
                        TryTransferInternal(1, 0);
        if (count >= 3) TryTransferInternal(2, 0);
        if (count >= 4) TryTransferInternal(3, 0);
    }

    // Getters
    public bool IsMerger2() { return (MachineTypeIndex == MachineCollection.MERGER_2.Index); }
    public bool IsMerger3() { return (MachineTypeIndex == MachineCollection.MERGER_3.Index); }
    public bool IsMerger4() { return (MachineTypeIndex == MachineCollection.MERGER_4.Index); }

    private int Count()
    {
        return (Height() - 1) / 2;
    }

    private int Height()
    {
        if ( IsMerger2() )  return 5;
        if ( IsMerger3() )  return 7;
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
        DebugFile.Log("MachineMerger", indentation);
        indentation++;

        base.Debug(indentation);
    }
}
