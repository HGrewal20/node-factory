using System.Collections.Generic;
using UnityEngine;

public class HudShop : MonoBehaviour
{
    // Member Variables
    [SerializeField] private          HudShopEntry       prefab;
                     private readonly List<HudShopEntry> entries = new();

    // Build
    public void Build(Transform content)
    {
        int count = ShopCollection.Count();
        for (byte i = 0; i < count; i++)
        {
            ShopEntry shop = ShopCollection.Get(i);

            HudShopEntry entry = Instantiate(prefab, content);
            entry.Setup(shop);
            entries.Add(entry);
        }
    }

    // Update
    public void Refresh()
    {
        foreach (HudShopEntry entry in entries)
            entry.Refresh();
    }

    // Clear
    public void Clear()
    {
        entries.Clear();
    }
}