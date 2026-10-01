using System.Collections.Generic;
using UnityEngine;

public class MachineNodes
{
    // Member Variables
    private readonly List<MachineNode> entries = new();

    // Constructor
    public MachineNodes(){}

    // Clear
    public void Clear()
    {
        entries.Clear();
    }

    // Add
    private MachineNode Add()
    {
        MachineNode entry = new MachineNode();
        entries.Add(entry);
        return entry;
    }
    public MachineNode Add(bool isInput, bool isEnabled, byte side, int offsetX, int offsetY, int max = 1, Item itemAllowed = null)
    {
        MachineNode entry = Add();
        entry.SetInput  (isInput         );
        entry.SetEnabled(isEnabled       );
        entry.SetSide   (side            );
        entry.SetOffset (offsetX, offsetY);
        entry.SetAllowed(itemAllowed, max);
        return entry;
    }
    public void AddInputsLeft                (bool isEnabled, int count,              int max = 1, Item itemAllowed = null) { AddMultiple( true, isEnabled, count,       0, max, itemAllowed, MachineNode.SIDE_LEFT ); }
    public void AddOutputsRight              (bool isEnabled, int count, int offsetX, int max = 1, Item itemAllowed = null) { AddMultiple(false, isEnabled, count, offsetX, max, itemAllowed, MachineNode.SIDE_RIGHT); }
    public void AddMultiple    (bool isInput, bool isEnabled, int count, int offsetX, int max = 1, Item itemAllowed = null, byte side = MachineNode.SIDE_NONE)
    {
        int offsetY = 1;

        for (int i = 0; i < count; i++)
        {
            Add(isInput, isEnabled, side, offsetX, offsetY, max, itemAllowed);
            offsetY += 2;
        }
    }

    // Both
    public int Count()
    {
        return entries.Count;
    }

    public MachineNode Get(int index)
    {
        return entries[index];
    }

    public bool IsAllFull()
    {
        foreach (MachineNode entry in entries)
        {
            if ( !entry.IsEnabled )   continue;
            if ( !entry.IsFull()  )
                return false;
        }

        return true;
    }

    public bool IsAllEmpty()
    {
        foreach (MachineNode entry in entries)
        {
            if ( !entry.IsEnabled )   continue;
            if ( !entry.IsEmpty() )
                return false;
        }

        return true;
    }

    public bool Contains(MachineNode entry)
    {
        return entries.Contains(entry);
    }

    public int IndexOf(MachineNode node)
    {
        return entries.IndexOf(node);
    }

    // Input
    public void InputEmpty()
    {
        foreach (MachineNode entry in entries)
            entry.InputEmpty();
    }

    // Output
    public void Output(ItemAmounts amounts)
    {
        int count = amounts.Count();
        for (int i = 0; i < count; i++)
            entries[i].OutputSet( amounts.Get(i) );
    }

    // Processor
    public void DisableAll()
    {
        foreach (MachineNode entry in entries)
            entry.Disable();
    }

    public void SetAccepts(ItemAmounts amounts)
    {
        int count = amounts.Count();
        for (int i = 0; i < count; i++)
            entries[i].SetAccepts( amounts.Get(i) );
    }

    // Merger & Splitter
    public void OnlyKeepFirst()
    {
        if (entries.Count > 1)
            entries.RemoveRange(1, entries.Count - 1);
    }

    // Disconnect
    public void Disconnect()
    {
        foreach (MachineNode entry in entries)
            entry.Disconnect();
    }

    // Get
    public MachineNode Get(Vector2Int offset)
    {
        foreach (MachineNode entry in entries)
            if (entry.Offset == offset)
                return entry;

        return null;
    }

    // Link
    public void Link(GameLinks links)
    {
        foreach (MachineNode entry in entries)
            entry.Link(links);
    }

    // IO
    public void Save(IOWriter writer)
    {
        int count = Count();
        writer.WriteInt(count);

        foreach (MachineNode entry in entries)
            entry.Save(writer);
    }
    public void Load(IOReader reader)
    {
        entries.Clear();

        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
            Add().Load(reader);
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MachineNodes", indentation);
        indentation++;
        
        foreach (MachineNode entry in entries)
            entry.Debug(indentation);
    }
}
