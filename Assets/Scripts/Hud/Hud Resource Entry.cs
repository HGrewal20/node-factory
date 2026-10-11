using TMPro;
using UnityEngine;

public class HudResourceEntry : MonoBehaviour
{
    // Member Variables
    [SerializeField] private TMP_Text text;
                     private Item     item;

    public void Setup(Item item)
    {
        this.item = item;
        Refresh();
    }

    public void Refresh()
    {
        ItemAmount itemAmount = GameData.INSTANCE.delivered.Get(item);
        int amount = (itemAmount == null) ? 0 : itemAmount.Amount;

        text.text = " " + amount + " - " + item.Name;
    }
}