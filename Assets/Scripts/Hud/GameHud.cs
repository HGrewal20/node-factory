using UnityEngine;

public class GameHud : MonoBehaviour
{
    private bool shopOpen = false;

    public void ResetHud()
    {
        shopOpen = false;
    }

    public void DrawHud()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() || data.IsPaused )
            return;

        HandleToggleKey();
        DrawStatusPanel(data);

        if (shopOpen)
            DrawShopPanel(data);
    }

    private void HandleToggleKey()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.B)
        {
            shopOpen = !shopOpen;
            e.Use();
        }
    }

    private GUIStyle statusBodyStyle;
    private GUIStyle statusCompleteStyle;

    private void DrawStatusPanel(GameData data)
    {
        EnsureStatusStyles();

        string text = BuildStatusText(data);

        float w = Mathf.Clamp(Screen.width * 0.28f, 260f, 320f);
        float h = 34f + (CountLines(text) * 18f);
        Rect rect = new Rect(Screen.width - w - 10f, 10f, w, h);

        UiTheme.DrawPanel(rect);

        Rect inner = new Rect(rect.x + 16f, rect.y + 8f, rect.width - 32f, rect.height - 16f);
        GUIStyle style = data.IsCompleted ? statusCompleteStyle : statusBodyStyle;
        GUI.Label(inner, text, style);
    }

    private void EnsureStatusStyles()
    {
        if (statusBodyStyle != null)
            return;

        statusBodyStyle     = UiTheme.MakeLabelStyle(13, FontStyle.Normal, new Color(0.85f, 0.90f, 0.92f), richText: true);
        statusCompleteStyle = UiTheme.MakeLabelStyle(13, FontStyle.Normal, new Color(0.45f, 1f, 0.55f)    , richText: true);
    }

    private string BuildStatusText(GameData data)
    {
        string text = "<color=#4DD9F2><b>RESOURCES</b></color>\n" + ItemAmountsToString(data.delivered);
        text += "\n<color=#4DD9F2><b>AVAILABLE MACHINES</b></color>\n" + AvailableMachinesToString(data);
        text += $"\nTime: {data.TimeElapsed:F1}s   Completion: {data.Completion * 100f:F0}%";
        text += "\n\nPress B for shop";

        if (data.IsCompleted)
            text += "\n>>> LEVEL COMPLETE! <<<";

        return text;
    }

    private string ItemAmountsToString(ItemAmounts amounts)
    {
        int count = amounts.Count();
        if (count == 0)
            return "  (none yet)";

        string text = "";
        for (int i = 0; i < count; i++)
        {
            ItemAmount entry = amounts.Get(i);
            text += $"  {entry.Item.Name}: {entry.Amount}\n";
        }
        return text.TrimEnd('\n');
    }

    private string AvailableMachinesToString(GameData data)
    {
        string text = "";
        bool   any  = false;

        for (byte i = 0; i < MachineCollection.Count(); i++)
        {
            int amount = data.machinesAvailable.Amount(i);
            if (amount <= 0)
                continue;

            any   = true;
            text += $"  {MachineCollection.Get(i).Name}: {amount}\n";
        }

        return any ? text.TrimEnd('\n') : "  (none, buy some below)";
    }

    private int CountLines(string text)
    {
        int count = 1;
        foreach (char c in text)
            if (c == '\n')
                count++;
        return count;
    }

    private void DrawShopPanel(GameData data)
    {
        int   entryCount = ShopCollection.Count();
        float w          = 320f;
        float rowH       = 26f;
        float h          = 40f + (entryCount * rowH);
        Rect  rect       = new Rect(10f, Screen.height - h - 10f, w, h);

        GUI.color = new Color(0f, 0f, 0f, 0.75f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);

        GUI.color = Color.white;
        GUI.Label(new Rect(rect.x + 8f, rect.y + 4f, rect.width - 16f, 20f), "SHOP (B to close)");

        for (byte i = 0; i < entryCount; i++)
        {
            ShopEntry entry = ShopCollection.Get(i);
            float     y     = rect.y + 28f + (i * rowH);

            string label = entry.machineType.Name + " - " + CostToString(entry.cost);
            GUI.Label(new Rect(rect.x + 8f, y, 220f, rowH), label);

            bool canAfford = data.delivered.IsMet(entry.cost);
            GUI.enabled = canAfford;
            if ( GUI.Button(new Rect(rect.x + rect.width - 70f, y, 60f, 22f), "Buy") )
            {
                data.delivered.TrySubtract(entry.cost);
                data.machinesAvailable.Modify(entry.machineType, 1);
            }
            GUI.enabled = true;
        }
    }

    private string CostToString(ItemAmounts cost)
    {
        int    count = cost.Count();
        string text  = "";
        for (int i = 0; i < count; i++)
        {
            ItemAmount entry = cost.Get(i);
            text += entry.Item.Name + " x" + entry.Amount;
            if (i < count - 1)
                text += ", ";
        }
        return text;
    }
}
