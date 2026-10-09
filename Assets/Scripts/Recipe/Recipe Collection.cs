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

    // Entries - All - Circuit Theme (Michael)
    public static readonly Recipe MINE_SILICON = Add(0, "Mine Silicon", 1, 
                                                    null,
                                                    new ItemAmounts().Add(ItemCollection.SILICON, 1) );
    public static readonly Recipe REFINE_WAFER = Add(1, "Refine Wafer", 2, 
                                                    new ItemAmounts().Add(ItemCollection.SILICON, 1),
                                                    new ItemAmounts().Add(ItemCollection.WAFER  , 1) );
    public static readonly Recipe MINE_COPPER  = Add(2, "Mine Copper" , 2, 
                                                    null,
                                                    new ItemAmounts().Add(ItemCollection.COPPER , 1) );
    public static readonly Recipe DRAW_TRACE   = Add(3, "Draw Trace"  , 1, 
                                                    new ItemAmounts().Add(ItemCollection.COPPER , 1),
                                                    new ItemAmounts().Add(ItemCollection.TRACE  , 2) );
    public static readonly Recipe ETCH_CHIP    = Add(4, "Etch Chip"   , 4, 
                                                    new ItemAmounts().Add(ItemCollection.WAFER  , 3),
                                                    new ItemAmounts().Add(ItemCollection.CHIP   , 1) );
    public static readonly Recipe PRINT_BOARD  = Add(5, "Print Board" , 5, 
                                                    new ItemAmounts().Add(ItemCollection.TRACE  , 4),
                                                    new ItemAmounts().Add(ItemCollection.BOARD  , 1) );

    // Entries - Machine Specific (First recipe is the default)
    public static readonly Recipes RECIPES_MINER   = new Recipes()
                                                        .Add(MINE_SILICON)
                                                        .Add(MINE_COPPER );
    public static readonly Recipes RECIPES_SMELTER = new Recipes()
                                                        .Add(REFINE_WAFER)
                                                        .Add(DRAW_TRACE  )
                                                        .Add(ETCH_CHIP   )
                                                        .Add(PRINT_BOARD );
}
