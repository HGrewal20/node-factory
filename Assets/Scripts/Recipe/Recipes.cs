using System.Collections.Generic;

public class Recipes
{
    // Member Variabels
    private readonly List<Recipe> entries = new();  // Does not allow duplicates

    // Constructor
    public Recipes(){}

    // Add
    public Recipes Add(Recipe entry)
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

    public Recipe Get(int index)
    {
        return entries[index];
    }

    public Recipe Get(Recipe other)
    {
        foreach (Recipe entry in entries)
            if (entry == other)
                return entry;

        return null;
    }

    public bool IsContained(Recipe other)
    {
        return Get(other) != null;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Recipes", indentation);
        indentation++;

        foreach (Recipe entry in entries)
            entry.Debug(indentation);
    }
}
