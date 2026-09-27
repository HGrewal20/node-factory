public class ItemRequirement
{
    // Member Variables
    public int  Amount   { get; private set; }  // >= 0
    public int  Required { get; private set; }  // >= 0
    public Item Item     { get; private set; }  // Should not be null, no safety check

    // Constructor
    public ItemRequirement(){}

    // Set
    public void Set(int amount, int required, Item item)
    {
        if (amount   < 0) amount   = 0;
        if (required < 0) required = 0;

        Amount   = amount;
        Required = required;
        Item     = item;
    }

    // Get
    public bool IsMet()
    {
        return (Amount >= Required);
    }

    public float GetCompletion()
    {
        if (Required == 0)
            return 1f;

        float completion = (float) (Amount / Required);
        if (completion > 1f)
            completion = 1f;
        return completion;
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("ItemRequirement", indentation);
        indentation++;

        DebugFile.Log("Amount:   " + Amount  , indentation);
        DebugFile.Log("Required: " + Required, indentation);

        if (Item != null)   Item.Debug(indentation);
        else                DebugFile.Log("Item: null", indentation);
    }
}
