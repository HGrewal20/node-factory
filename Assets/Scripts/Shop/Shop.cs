using System.Collections.Generic;

// Note:
// Probably won't use this class but it is complete so leave it
// Useful if we want to allow different purchasing options per level
public class Shop
{
    // Member Variables
    private readonly List<ShopEntry> entries = new();   // Does not allow duplicates

    // Constructor
    public Shop(){}

    // Add
    public Shop Add(ShopEntry entry)
    {
        if ( !IsContained(entry) )
            entries.Add(entry);
        return this;
    }

    // Get
    public int Count()
    {
        return entries.Count;
    }

    public ShopEntry Get(int index)
    {
        return entries[index];
    }

    public ShopEntry Get(ShopEntry other)
    {
        foreach (ShopEntry entry in entries)
            if (entry == other)
                return entry;

        return null;
    }

    public bool IsContained(ShopEntry other)
    {
        return Get(other) != null;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Shop", indentation);
        indentation++;

        foreach (ShopEntry entry in entries)
            entry.Debug(indentation);
    }
}
