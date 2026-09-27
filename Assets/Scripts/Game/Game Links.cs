using System.Collections.Generic;

public class GameLinks
{
    // Instance
    public static readonly GameLinks INSTANCE = new GameLinks();

    // Member Variables
    private Dictionary<long, Machine> machineMap = new Dictionary<long, Machine>();

    // Constructor
    private GameLinks(){}

    // Clear
    public void Clear()
    {
        machineMap.Clear();
    }

    // Fill
    public void Fill(GameData data)
    {
        Clear();
        Fill(data.machinesUsed);
    }

    private void Fill(Machines machines)
    {
        int count = machines.Count();
        for (int i = 0; i < count; i++)
        {
            Machine machine = machines.Get(i);
            machineMap[machine.ID] = machine;
        }
    }

    // Get
    public Machine GetMachine(long id)
    {
        if (machineMap.TryGetValue(id, out Machine machine))
            return machine;

        return null;
    }
    
    public MachineWire GetWire(long id)
    {
        Machine machine = GetMachine(id);
        if (machine == null) return null;
        else                 return machine as MachineWire;
    }
}
