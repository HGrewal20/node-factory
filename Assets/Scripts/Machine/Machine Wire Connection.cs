public class MachineWireConnection
{
    // Member Variables
    public  bool    IsInput     { get; private set; }   // Only set on creation
    public  int     NodeIndex   { get; private set; }
    private long    MachineID;                          // Only used to load and link
    public  Machine Machine     { get; private set; }
    
    // Constructor
    public MachineWireConnection(bool isInput)
    {
        IsInput = isInput;
    }

    // Connect

    // Disconnect
    public void Disconnect()
    {
        MachineNode node = GetNode();
        if (node != null)
        {
            node.SetWire(null);
            Set(0, null);
        } 
    }

    // Set
    public void Set(int nodeIndex, Machine machine)
    {
        NodeIndex = nodeIndex;
        Machine   = machine;
    }

    // Get
    public MachineNode GetNode()
    {
        MachineNodes nodes = GetNodes();
        if (nodes == null)      return null;
        else                    return nodes.Get(NodeIndex);
    }
    public MachineNodes GetNodes()
    {
        if (Machine == null)    return null;
        if (IsInput        )    return Machine.inputs;
        else                    return Machine.outputs;
    }

    // Link
    public void Link(GameLinks links)
    {
        Machine = links.GetMachine(MachineID);
    }

    // IO
    public void Save(IOWriter writer)
    {
        long machineID = (Machine == null) ? 0 : Machine.ID;

        writer.WriteBool(IsInput  );
        writer.WriteInt (NodeIndex);
        writer.WriteLong(machineID);
    }
    public void Load(IOReader reader)
    {
        IsInput   = reader.ReadBool();
        NodeIndex = reader.ReadInt ();
        MachineID = reader.ReadLong();
    }

    // Debug
    public void Debug(int indentation)
    {
        long machineID = (Machine == null) ? 0 : Machine.ID;

        DebugFile.Log("MachineWireNode", indentation);
        indentation++;

        DebugFile.Log("IsInput:   " + IsInput  , indentation);
        DebugFile.Log("NodeIndex: " + NodeIndex, indentation);
        DebugFile.Log("MachineID: " + MachineID, indentation);
        DebugFile.Log("machineID: " + machineID, indentation);
    }
}
