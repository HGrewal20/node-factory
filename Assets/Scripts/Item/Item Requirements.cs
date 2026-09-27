using System.Collections.Generic;

public class ItemRequirements
{
    // Instance
    public static readonly ItemRequirements INSTANCE = new ItemRequirements();

    // Member Variables
    private readonly List<ItemRequirement> entries   = new List<ItemRequirement>();
    private          int                   usedCount = 0;

    // Constructor
    private ItemRequirements(){}

    // Clear
    public void Clear()
    {
        usedCount = 0;
    }

    // Get
    public int Count()
    {
        return usedCount;
    }

    public ItemRequirement Get(int index)
    {
        return entries[index];
    }

    public bool IsAllMet()
    {
        for (int i = 0; i < usedCount; i++)
            if ( !entries[i].IsMet() )
                return false;

        return true;
    }

    public float GetCompletion()
    {
        if (usedCount == 0)
            return 1;

        float total = 0f;

        for (int i = 0; i < usedCount; i++)
            total += entries[i].GetCompletion();

        return (total / usedCount);
    }

    // Set
    public void Set(ItemAmounts amountsHave, ItemAmounts amountsNeeded)
    {
        int count = amountsNeeded.Count();

        usedCount = count;

        // Make sure enough entries exist.
        while (entries.Count < usedCount)
            entries.Add(new ItemRequirement());

        // Only update entries currently in use.
        for (int i = 0; i < usedCount; i++)
        {
            ItemAmount amountNeeded = amountsNeeded.Get(i);
            ItemAmount amountHave   = amountsHave  .Get(amountNeeded.Item);

            int have = (amountHave == null) ? 0 : amountHave.Amount;

            entries[i].Set(have, amountNeeded.Amount, amountNeeded.Item);
        }
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("ItemRequirements", indentation);
        indentation++;

        for (int i = 0; i < usedCount; i++)
            entries[i].Debug(indentation);
    }
}