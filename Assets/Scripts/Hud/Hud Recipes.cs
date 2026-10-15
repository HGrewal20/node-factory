using System.Collections.Generic;
using UnityEngine;

public class HudRecipes : MonoBehaviour
{
    // References
    [SerializeField] private Transform        content;
    [SerializeField] private HudRecipe        recipePrefab;
    [SerializeField] private HudRecipeDetails details;

    // State
    private          MachineProcessor machine;
    private readonly List<HudRecipe>  entries = new();

    private void Awake()
    {
        details.gameObject.SetActive(false);
           this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (machine == null)
            return;

        for (int i = 0; i < entries.Count; i++)
            entries[i].SetSelected(entries[i].Recipe == machine.Recipe);
    }

    // Set - Safe to call every frame
    public void Set(MachineProcessor processor)
    {
        if (processor == null)
        {
            TryClose();
            return;
        }

        if (machine != processor)
        {
            machine = processor;
            Rebuild();
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    private void Rebuild()
    {
        foreach (HudRecipe entry in entries)
            if (entry != null)
                Destroy(entry.gameObject);

        entries.Clear();
        details.gameObject.SetActive(false);

        Recipes recipes = machine.GetRecipes();
        if (recipes == null)
            return;

        for (int i = 0; i < recipes.Count(); i++)
        {
            Recipe recipe = recipes.Get(i);

            int index = i;

            HudRecipe entry = Instantiate(recipePrefab, content);
            entry.Set(recipe,
                r => Select(index),
                r =>
                {
                    details.Set(r);
                    details.gameObject.SetActive(true);
                },
                r => details.gameObject.SetActive(false));

            entries.Add(entry);
        }
    }

    private void Select(int index)
    {
        if (machine == null)
            return;

        machine.SetRecipe(index);
        Set(machine);
    }

    public void ButtonClose()
    {
        TryClose();
    }

    public bool TryClose()
    {
        if (!gameObject.activeSelf)
            return false;

        machine = null;
        details.gameObject.SetActive(false);
        gameObject.SetActive(false);
        return true;
    }
}