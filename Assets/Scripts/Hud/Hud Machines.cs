using System.Collections.Generic;
using UnityEngine;

public class HudMachines : MonoBehaviour
{
    // Member Variables
    [SerializeField] private          HudMachineEntry       prefab;
    [SerializeField] private          GameInteraction       interaction;
                     private readonly List<HudMachineEntry> entries         = new();

    // Build
    public void Build(Transform content)
    {
        int count = MachineCollection.Count();
        for (byte i = 0; i < count; i++)
        {
            MachineType machine = MachineCollection.Get(i);

            HudMachineEntry entry = Instantiate(prefab, content);
            entry.Setup(machine, interaction);
            entries.Add(entry);
        }
    }

    // Update
    public void Refresh()
    {
        foreach (HudMachineEntry entry in entries)
            entry.Refresh();
    }

    // Clear
    public void Clear()
    {
        entries.Clear();
    }
}