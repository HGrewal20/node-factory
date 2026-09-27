public class ItemAmount
{
    // Member Variables
    public Item Item   { get; private set; }
    public int  Amount { get; private set; }    // Cannot be negative

    // Constructor
    public ItemAmount(){}
    public ItemAmount(Item item, int amount)
    {
        Item = item;
        TrySetAmount(amount);
    }
    public ItemAmount(ItemAmount other)
    {
        Item   = other.Item;
        Amount = other.Amount;
    }

    // Set
    private bool TrySetAmount(int amount)
    {
        if (amount >= 0)
        {
            Amount = amount;
            return true;
        }
        return false;
    }
    public bool TryModifyAmount(int delta)
    {
        return TrySetAmount(Amount + delta);
    }

    // IO
    public void Save(IOWriter writer)
    {
        writer.WriteItem(Item  );
        writer.WriteInt (Amount);
    }
    public void Load(IOReader reader)
    {
        Item   = reader.ReadItem();
        Amount = reader.ReadInt ();
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("ItemAmount", indentation);
        indentation++;

        if (Item != null)   Item.Debug(indentation);
        else                DebugFile.Log("Item: null", indentation);

        DebugFile.Log("Amount: " + Amount, indentation);
    }
}
