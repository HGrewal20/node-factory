public class MachineDelivery : Machine
{
    // Constructor
    public MachineDelivery(byte machineTypeIndex, long id)
        : base(machineTypeIndex, id)
    {
        base.SetSize(3, 3);
        inputs.AddInputsLeft(true, 1, 1, null);
    }

    // Game Tick
    public override void GameTick()
    {
        base.GameTick();

        MachineNode node = inputs.Get(0);
        if ( !node.IsEmpty() )
        {
            Item item   = node.ItemCurrent;
            int  amount = node.Amount;
            node.InputEmpty();

            GameData.INSTANCE.delivered.Add(item, amount);
        }
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
        DebugFile.Log("MachineDelivery", indentation);
        indentation++;

        base.Debug(indentation);
    }
}
