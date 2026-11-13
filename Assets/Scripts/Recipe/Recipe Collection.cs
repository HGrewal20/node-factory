using System;
using System.Collections.Generic;

public static class RecipeCollection
{
    // Collection
    private static readonly List<Recipe> ENTRIES = new();
    private static Recipe Add(String name, int timeSecs, ItemAmounts inputs, ItemAmounts outputs)
    {
        byte index = (byte) ENTRIES.Count;
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

    // Entries - All - Food Factory

    // Farm - Produces Resources
    public static readonly Recipe GROW_WHEAT = Add("Grow Wheat", 1,
        null,
        new ItemAmounts().Add(ItemCollection.WHEAT, 1));

    public static readonly Recipe GROW_BERRIES = Add("Grow Berries", 2,
        null,
        new ItemAmounts().Add(ItemCollection.BERRIES, 1));

    public static readonly Recipe COLLECT_MILK = Add("Collect Milk", 2,
        null,
        new ItemAmounts().Add(ItemCollection.MILK, 1));

    // Mixer - Processes Resources
    public static readonly Recipe MILL_FLOUR = Add("Mill Flour", 2,
        new ItemAmounts().Add(ItemCollection.WHEAT, 1),
        new ItemAmounts().Add(ItemCollection.FLOUR, 1));

    public static readonly Recipe MAKE_JAM = Add("Make Jam", 3,
        new ItemAmounts().Add(ItemCollection.BERRIES, 1),
        new ItemAmounts().Add(ItemCollection.JAM, 1));

    public static readonly Recipe MAKE_CREAM = Add("Make Cream", 3,
        new ItemAmounts().Add(ItemCollection.MILK, 1),
        new ItemAmounts().Add(ItemCollection.CREAM, 1));

    // Oven - Produces Finished Food
    public static readonly Recipe BAKE_BREAD = Add("Bake Bread", 3,
        new ItemAmounts().Add(ItemCollection.FLOUR, 1),
        new ItemAmounts().Add(ItemCollection.BREAD, 1));

    public static readonly Recipe BAKE_TART = Add("Bake Tart", 4,
        new ItemAmounts()
            .Add(ItemCollection.FLOUR, 1)
            .Add(ItemCollection.JAM, 1),
        new ItemAmounts().Add(ItemCollection.TART, 1));

    public static readonly Recipe BAKE_CAKE = Add("Bake Cake", 5,
        new ItemAmounts()
            .Add(ItemCollection.FLOUR, 1)
            .Add(ItemCollection.CREAM, 1),
        new ItemAmounts().Add(ItemCollection.CAKE, 1));

    // Entries - Machine Specific
    // First Recipe is Default
    public static readonly Recipes RECIPES_FARM = new Recipes()
        .Add(GROW_WHEAT)
        .Add(GROW_BERRIES)
        .Add(COLLECT_MILK);

    public static readonly Recipes RECIPES_MIXER = new Recipes()
        .Add(MILL_FLOUR)
        .Add(MAKE_JAM)
        .Add(MAKE_CREAM);

    public static readonly Recipes RECIPES_OVEN = new Recipes()
        .Add(BAKE_BREAD)
        .Add(BAKE_TART)
        .Add(BAKE_CAKE);
}
