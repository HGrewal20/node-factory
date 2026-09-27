using System;
using UnityEngine;

public class Recipe
{
    // Member Variables
    public          byte        Index    { get; private set; }
    public          String      Name     { get; private set; }
    public          int         TimeSecs { get; private set; }  // Cannot be less than 1
    public readonly ItemAmounts inputs;
    public readonly ItemAmounts outputs;
    
    // Constructor
    public Recipe(byte index, String name, int timeSecs, ItemAmounts inputs, ItemAmounts outputs)
    {
        this.Index    = index;
        this.Name     = name;
        this.TimeSecs = Mathf.Max(1, timeSecs);
        this.inputs   = (inputs  == null) ? new ItemAmounts() : inputs;
        this.outputs  = (outputs == null) ? new ItemAmounts() : outputs;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("Recipe", indentation);
        indentation++;

        DebugFile.Log("Index:    " + Index,    indentation);
        DebugFile.Log("Name:     " + Name,     indentation);
        DebugFile.Log("TimeSecs: " + TimeSecs, indentation);

        inputs .Debug(indentation);
        outputs.Debug(indentation);
    }
}
