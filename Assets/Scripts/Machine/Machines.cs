using System.Collections.Generic;

public class Machines
{
    // Member Variables
    private          long          idFree  = 0;
    private readonly List<Machine> entries = new();     // No Duplicates

    // Constructor
    public Machines(){}

    // Clear
    public void Clear()
    {
        idFree = 0;
        entries.Clear();
    }

    // Add
    public Machine Add(byte machineTypeIndex)
    {
        Machine entry = MachineConstructor.Create(machineTypeIndex, GetNextFreeID() );
        entries.Add(entry);
        return entry;
    }
    
    private long GetNextFreeID()
    {
        ++idFree;
        return idFree;
    }

    // Remove
    public void Remove(Machine entry)
    {
        entries.Remove(entry);
    }

    // Get
    public int Count()
    {
        return entries.Count;
    }

    public Machine Get(int index)
    {
        return entries[index];
    }

    public Machine Get(Machine other)
    {
        foreach (Machine entry in entries)
            if (entry == other)
                return entry;

        return null;
    }

    public bool IsContained(Machine other)
    {
        return Get(other) != null;
    }

    // Game Tick
    public void GameTick()
    {
        foreach (Machine entry in entries)
            entry.GameTick();
    }

    // Link
    public void Link(GameLinks links)
    {
        foreach (Machine entry in entries)
            entry.Link(links);
    }

    // IO
    public void Save(IOWriter writer)
    {
        writer.WriteLong(idFree);

        int count = entries.Count;
        writer.WriteInt(count);

        foreach (Machine entry in entries)
            entry.Save(writer);
    }
    public void Load(IOReader reader)
    {
        entries.Clear();

        idFree = reader.ReadLong();

        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            byte machineTypeIndex = reader.ReadByte();
            long id               = reader.ReadLong();

            Machine entry = MachineConstructor.Create(machineTypeIndex, id);
            entry.Load(reader);
            entries.Add(entry);
        }
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Machines", indentation);
        indentation++;

        DebugFile.Log("idFree = " + idFree, indentation);

        foreach (Machine entry in entries)
            entry.Debug(indentation);
    }
}
