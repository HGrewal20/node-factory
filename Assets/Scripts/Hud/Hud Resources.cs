using System.Collections.Generic;
using UnityEngine;

public class HudResources : MonoBehaviour
{
    // Member Variables
    [SerializeField] private          HudResourceEntry          prefab;
                     private readonly List<HudResourceEntry>    entries         = new();
                     private          int                       resourceCount;

    // Build
    public void Build(Transform content)
    {
        ItemAmounts delivered = GameData.INSTANCE.delivered;
        resourceCount = delivered.Count();

        for (int i = 0; i < resourceCount; i++)
        {
            Item item = delivered.Get(i).Item;

            HudResourceEntry entry = Instantiate(prefab, content);
            entry.Setup(item);
            entries.Add(entry);
        }
    }

    // Update
    public bool NeedsRebuild()
    {
        return GameData.INSTANCE.delivered.Count() != resourceCount;
    }

    public void Refresh()
    {
        foreach (HudResourceEntry entry in entries)
            entry.Refresh();
    }

    // Clear
    public void Clear()
    {
        entries       .Clear();
        resourceCount = 0;
    }
}