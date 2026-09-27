public class ShopEntry
{
    // Member Variables
    public readonly MachineType machineType;
    public readonly ItemAmounts cost        = new ItemAmounts();
    
    // Constructor
    public ShopEntry(MachineType machineType)
    {
        this.machineType = machineType;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("ShopEntry", indentation);
        indentation++;

        machineType.Debug(indentation);
        cost       .Debug(indentation);
    }
}
