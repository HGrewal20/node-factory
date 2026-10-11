using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudShopEntry : MonoBehaviour
{
    // Constants
    private static readonly Color colorMet     = Color.green;
    private static readonly Color colorMissing = Color.red;

    // Member Variables
    [SerializeField] private TMP_Text   textMachine;
    [SerializeField] private TMP_Text[] textCosts;
    [SerializeField] private Button     button;
                     private ShopEntry  entry;

    // Setup
    public void Setup(ShopEntry entry)
    {
        this.entry = entry;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener       (Buy);

        Refresh();
    }

    // Refresh
    public void Refresh()
    {
        GameData data = GameData.INSTANCE;

        // Machine Amount and Type
        int available = data.machinesAvailable.Amount(entry.machineType.Index);
        textMachine.text = " " + available + " - " + entry.machineType.Name;

        // Cost Entries
        int count = Mathf.Min(entry.cost.Count(), textCosts.Length);
        for (int i = 0; i < textCosts.Length; i++)
        {
            // Set Visible
            bool visible = i < count;
            textCosts[i].gameObject.SetActive(visible);

            if (!visible)
            {
                textCosts[i].text = "";
                continue;
            }

            // Set Cost Text and Color
            ItemAmount cost = entry.cost.Get(i);
            ItemAmount have = data.delivered.Get(cost.Item);

            int amountHave = (have == null) ? 0 : have.Amount;

            textCosts[i].text  = " " + cost.Amount + "X " + cost.Item.Name;
            textCosts[i].color = (amountHave >= cost.Amount) ? colorMet : colorMissing;
        }

        // Button
        button.interactable = data.delivered.IsMet(entry.cost);
    }

    // Buy
    private void Buy()
    {
        GameData data = GameData.INSTANCE;

        if (data.delivered.TrySubtract(entry.cost))
            data.machinesAvailable.Modify(entry.machineType, 1);

        Refresh();
    }
}