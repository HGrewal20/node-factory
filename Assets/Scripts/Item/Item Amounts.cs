using System.Collections.Generic;

public class ItemAmounts
{
    // Member Variables
    private readonly List<ItemAmount> entries = new();  // Item never null, amount always > 0, no duplicate items

    // Constructor
    public ItemAmounts(){}

    // Clear
    public void Clear()
    {
        entries.Clear();
    }

    // Set
    public void Set(ItemAmounts other)
    {
        entries.Clear();

        foreach (ItemAmount entry in other.entries)
            entries.Add( new ItemAmount(entry) );
    }

    // Add
    public ItemAmounts Add(ItemAmount entry) { return Add(entry.Item, entry.Amount); }
    public ItemAmounts Add(Item item, int amount)
    {
        if (item == null || amount <= 0)
            return this;

        ItemAmount current = Get(item);
        if (current != null)    current.TryModifyAmount(amount);                // If Already Exists -> Increment Amount. Guarenteed to work.
        else                    entries.Add( new ItemAmount(item, amount) );    // If New -> Add New Entry
        return this;
    }

    // Subtract
    public bool TrySubtract(ItemAmount entry)
    {
        ItemAmount current = Get(entry.Item);
        if (current != null)
            return current.TryModifyAmount(-entry.Amount);
        return false;
    }

    public bool TrySubtract(ItemAmounts other)
    {
        if ( IsMet(other) )
        {
            foreach (ItemAmount entry in other.entries)
                TrySubtract(entry);     // Guarenteed to work
            
            return true;
        }
        return false;
    }

    // Get
    public int Count()
    {
        return entries.Count;
    }

    public ItemAmount Get(int index)
    {
        return entries[index];
    }
    public ItemAmount Get(Item item)
    {
        foreach (ItemAmount entry in entries)
            if (entry.Item == item)
                return entry;
        
        return null;
    }

    public bool IsMet(ItemAmounts requirements)
    {
        foreach (ItemAmount entryRequirements in requirements.entries)
        {
            // If None -> Fail
            ItemAmount entryThis = Get(entryRequirements.Item);
            if (entryThis == null)
                return false;

            // If Not Enough -> Fail
            if (entryThis.Amount < entryRequirements.Amount)
                return false;
        }

        return true;
    }

    // IO
    public void Save(IOWriter writer)
    {
        int count = Count();
        writer.WriteInt(count);

        foreach (ItemAmount entry in entries)
            entry.Save(writer);
    }
    public void Load(IOReader reader)
    {
        entries.Clear();

        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            ItemAmount entry = new ItemAmount();
            entry.Load(reader);
            entries.Add(entry);
        }
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("ItemAmounts", indentation);
        indentation++;

        foreach (ItemAmount entry in entries)
            entry.Debug(indentation);
    }
}
