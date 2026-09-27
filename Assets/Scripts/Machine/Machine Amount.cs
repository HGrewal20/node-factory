// Note: Probably won't use this class but it is complete so leave it
public class MachineAmount
{
    // Member Variables
    public MachineType MachineType { get; private set; }
    public int         Amount      { get; private set; }

    // Constructor
    public MachineAmount(MachineType machineType, int amount)
    {
        MachineType = machineType;
        Amount      = amount;
    }

    // IO
    public void Save(IOWriter writer)
    {
        writer.WriteMachineType(MachineType);
        writer.WriteInt        (Amount     );
    }
    public void Load(IOReader reader)
    {
        MachineType = reader.ReadMachineType();
        Amount      = reader.ReadInt        ();
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MachineAmount", indentation);
        indentation++;

        MachineType.Debug(indentation);
        DebugFile.Log("Amount: " + Amount, indentation);
    }
}
