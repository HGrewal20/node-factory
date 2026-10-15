using System;
using UnityEngine;

public class HudRecipeDetails : MonoBehaviour
{
    // References
    [SerializeField] private HudLine[] lines;

    // Colors
    private static readonly Color COLOR_NAME   = Color.white;
    private static readonly Color COLOR_HEADER = new Color(0.3f, 0.7f, 1.0f);
    private static readonly Color COLOR_NORMAL = Color.white;

    // State
    private int nextLine;

    // Initialize
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    // Set
    public void Set(Recipe recipe)
    {
        // No Recipe
        if (recipe == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // Begin Display
        gameObject.SetActive(true);
        nextLine = 0;

        // Lines - Used
        AddLine (recipe.Name                    , COLOR_NAME    );
        AddLine ($"Time: {recipe.TimeSecs} sec" , COLOR_NORMAL  );
        AddItems("Inputs" , "-"                 , recipe.inputs );
        AddItems("Outputs", "+"                 , recipe.outputs);

        // Lines - Empty
        for (int i = nextLine; i < lines.Length; i++)
            lines[i].Hide();
    }

    // Add
    private void AddItems(string title, string prefix, ItemAmounts items)
    {
        if (items.Count() == 0)
            return;

        // Header
        AddLine(title, COLOR_HEADER);

        // Items
        for (int i = 0; i < items.Count(); i++)
        {
            ItemAmount entry = items.Get(i);
            AddLine($"{prefix}{entry.Amount} {entry.Item.Name}", COLOR_NORMAL);
        }
    }

    private void AddLine(string value, Color color)
    {
        if (nextLine < lines.Length)
        {
            lines[nextLine].Set(value, color);
            nextLine++;
        }
    }
}