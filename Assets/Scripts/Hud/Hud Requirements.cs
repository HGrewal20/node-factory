using System.Collections.Generic;
using UnityEngine;

public class HudRequirements : MonoBehaviour
{
    // Member Variables
    [SerializeField] private          HudRequirementEntry       prefab;
                     private readonly List<HudRequirementEntry> entries = new();

    // Build
    public void Build(Transform content)
    {
        int count = GameData.INSTANCE.Level.deliveryGoal.Count();
        for (int i = 0; i < count; i++)
        {
            HudRequirementEntry entry = Instantiate(prefab, content);
            entries.Add(entry);
        }

        Refresh();
    }

    // Update
    public void Refresh()
    {
        GameData data = GameData.INSTANCE;

        ItemRequirements requirements = ItemRequirements.INSTANCE;
        requirements.Set(data.delivered, data.Level.deliveryGoal);

        for (int i = 0; i < entries.Count; i++)
            entries[i].Set(requirements.Get(i));
    }

    // Clear
    public void Clear()
    {
        entries.Clear();
    }
}