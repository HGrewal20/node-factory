using System;
using System.Collections.Generic;

public static class RecipeCollection
{
    // Collection
    private static readonly List<Recipe> ENTRIES = new();
    private static Recipe Add(byte index, String name, int timeSecs, ItemAmounts inputs, ItemAmounts outputs)
    {
        Recipe entry = new Recipe(index, name, timeSecs, inputs, outputs);
        ENTRIES.Add(entry);
        return entry;
    }
    public static int Count()
    {
        return ENTRIES.Count;
    }
    public static Recipe Get(byte index)
    {
        return ENTRIES[index];
    }
    public static void Debug(int indentation)
    {
        DebugFile.Log("RecipeCollection", indentation);
        indentation++;

        foreach (Recipe entry in ENTRIES)
            entry.Debug(indentation);
    }

    // Entries - All
    public static readonly Recipe MINE_IRON_ORE    = Add(0, "Mine Iron Ore", 1, 
                                                        null,
                                                        new ItemAmounts().Add(ItemCollection.IRON_ORE  , 1) );
    public static readonly Recipe SMELT_IRON_PLATE = Add(1, "Iron Ore", 2, 
                                                        new ItemAmounts().Add(ItemCollection.IRON_ORE  , 1),
                                                        new ItemAmounts().Add(ItemCollection.IRON_PLATE, 1) );

    // Entries - Machine Specific
    public static readonly Recipes RECIPES_MINER   = new Recipes()
                                                        .Add(MINE_IRON_ORE)
                                                        .Add(MINE_IRON_ORE);        // Note: Illustrates usage only. Don't add the same recipe. Will block and do nothing. Remove later.
    public static readonly Recipes RECIPES_SMELTER = new Recipes()
                                                        .Add(SMELT_IRON_PLATE);
}
