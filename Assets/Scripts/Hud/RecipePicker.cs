using UnityEngine;

// Small popup for choosing what a Miner/Smelter makes: left click the
// machine's body (not one of its nodes) and a list of that machine's
// recipes appears. Pick one and it's applied with SetRecipe().
//
// This is the "recipe selection" UI Michael's DebugAutoRecipe.cs was
// standing in for. Opened by GameInteraction, drawn by GameHud.
public class RecipePicker
{
    private MachineProcessor target;

    private GUIStyle titleStyle;
    private float    stylesScale = -1f;

    public bool IsOpen { get { return target != null; } }

    public void Open(MachineProcessor processor) { target = processor; }
    public void Close()                          { target = null;      }

    public void Draw()
    {
        if (target == null)
            return;

        GameData data = GameData.INSTANCE;

        // Close if the game stopped/paused or the machine was removed.
        if ( !data.IsPlaying() || data.IsPaused || data.map.mapMachines.Get(target.Pos) != target )
        {
            target = null;
            return;
        }

        EnsureStyles();

        Recipes recipes = target.GetRecipes();
        int     count   = (recipes == null) ? 0 : recipes.Count();

        float pad    = UiTheme.S(16f);
        float gap    = UiTheme.S(6f);
        float rowH   = UiTheme.S(36f);
        float titleH = UiTheme.S(30f);
        float w      = Mathf.Min(Mathf.Clamp(Screen.width * 0.40f, UiTheme.S(380f), UiTheme.S(600f)), Screen.width * 0.9f);
        float h      = pad + titleH + count * (rowH + gap) + rowH + pad;

        Rect rect = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);

        // Clicking anywhere outside the popup closes it.
        Event e = Event.current;
        if (e.type == EventType.MouseDown && !rect.Contains(e.mousePosition))
        {
            target = null;
            return;
        }

        UiTheme.DrawPanel(rect);

        string name = MachineCollection.Get(target.MachineTypeIndex).Name;
        GUI.Label(new Rect(rect.x + pad, rect.y + pad * 0.6f, w - pad * 2f, titleH),
                  "<color=#4DD9F2><b>" + name + " recipe</b></color>", titleStyle);

        float y = rect.y + pad * 0.6f + titleH;

        for (int i = 0; i < count; i++)
        {
            Recipe recipe  = recipes.Get(i);
            bool   current = (target.Recipe == recipe);

            string label = (current ? "> " : "") + recipe.Name + "   " + AmountsToString(recipe.inputs) + " > " + AmountsToString(recipe.outputs) + "   " + recipe.TimeSecs + "s";

            if ( UiTheme.Button(new Rect(rect.x + pad, y, w - pad * 2f, rowH), label) )
            {
                target.SetRecipe(i);
                target = null;
                return;
            }
            y += rowH + gap;
        }

        if ( UiTheme.Button(new Rect(rect.x + pad, y, w - pad * 2f, rowH), "Close") )
            target = null;
    }

    private string AmountsToString(ItemAmounts amounts)
    {
        if (amounts == null || amounts.Count() == 0)
            return "(nothing)";

        string text = "";
        for (int i = 0; i < amounts.Count(); i++)
        {
            ItemAmount entry = amounts.Get(i);
            text += entry.Item.Name + " x" + entry.Amount;
            if (i < amounts.Count() - 1)
                text += ", ";
        }
        return text;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null && Mathf.Approximately(stylesScale, UiTheme.Scale))
            return;
        stylesScale = UiTheme.Scale;

        titleStyle = UiTheme.MakeLabelStyle(UiTheme.Font(16), FontStyle.Bold, Color.white, richText: true);
    }
}
