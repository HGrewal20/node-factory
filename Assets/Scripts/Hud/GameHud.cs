using UnityEngine;

// The in-game HUD: shows your current resources, the level goal, which
// machines you have available to place, the shop (buy new machines using
// delivered resources), and basic level progress (time, completion %).
//
// Press B (or the Shop button) to toggle the shop. The status panel (top
// right) is always visible while playing. Everything scales with the
// window height through UiTheme so the text stays readable.
public class GameHud : MonoBehaviour
{
    private bool    shopOpen = false;
    private Vector2 shopScroll;

    private GUIStyle statusBodyStyle;
    private GUIStyle statusCompleteStyle;
    private GUIStyle shopTitleStyle;
    private GUIStyle shopRowStyle;
    private GUIStyle popupTitleStyle;
    private GUIStyle popupBodyStyle;
    private bool     completeDismissed = false;
    private float    stylesScale = -1f;

    private readonly RecipePicker recipePicker = new RecipePicker();

    public void ToggleShop() { shopOpen = !shopOpen; }
    public void OpenRecipePicker(MachineProcessor processor) { recipePicker.Open(processor); }

    // Called by GameDisplay, after everything else it draws, so this
    // always ends up on top instead of racing with it for draw order
    // (Unity doesn't guarantee which component's OnGUI runs first).
    public void DrawHud()
    {
        GameData data = GameData.INSTANCE;
        if ( !data.IsPlaying() || GameMenu.IsPaused )
            return;

        EnsureStyles();
        HandleToggleKey();
        DrawStatusPanel(data);

        if (shopOpen)
            DrawShopPanel(data);

        recipePicker.Draw();
        DrawCompletePopup(data); // last, so it sits on top of everything
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

    // Styles depend on the window height (text scales), so rebuild them
    // whenever the scale changes, e.g. after resizing the Game view.
    private void EnsureStyles()
    {
        if (statusBodyStyle != null && Mathf.Approximately(stylesScale, UiTheme.Scale))
            return;
        stylesScale = UiTheme.Scale;

        Color body = new Color(0.85f, 0.90f, 0.92f);

        statusBodyStyle     = UiTheme.MakeLabelStyle(UiTheme.Font(14), FontStyle.Normal, body, richText: true);
        statusCompleteStyle = UiTheme.MakeLabelStyle(UiTheme.Font(14), FontStyle.Normal, new Color(0.45f, 1f, 0.55f), richText: true);
        shopTitleStyle      = UiTheme.MakeLabelStyle(UiTheme.Font(15), FontStyle.Bold  , body, richText: true);
        shopRowStyle        = UiTheme.MakeLabelStyle(UiTheme.Font(14), FontStyle.Normal, body, richText: true);
        shopRowStyle.alignment = TextAnchor.MiddleLeft;
        shopRowStyle.wordWrap  = false;

        popupTitleStyle = UiTheme.MakeLabelStyle(UiTheme.Font(30), FontStyle.BoldAndItalic, new Color(0.45f, 1f, 0.55f));
        popupTitleStyle.alignment = TextAnchor.UpperCenter;
        popupBodyStyle  = UiTheme.MakeLabelStyle(UiTheme.Font(16), FontStyle.Normal, body, richText: true);
        popupBodyStyle.alignment  = TextAnchor.UpperCenter;
    }

    // Top-right panel: resources you've delivered (this is your currency),
    // the level goal, machines you own but haven't placed yet, and progress.
    private void DrawStatusPanel(GameData data)
    {
        string   text  = BuildStatusText(data);
        GUIStyle style = IsReallyComplete(data) ? statusCompleteStyle : statusBodyStyle;

        float pad    = UiTheme.S(16f);
        float margin = UiTheme.S(10f);
        float w      = Mathf.Min(Mathf.Clamp(Screen.width * 0.26f, UiTheme.S(260f), UiTheme.S(380f)), Screen.width * 0.45f);
        float innerW = w - pad * 2f;
        float textH  = style.CalcHeight(new GUIContent(text), innerW);
        float h      = textH + pad;

        Rect rect = new Rect(Screen.width - w - margin, margin, w, h);
        UiTheme.DrawPanel(rect);

        GUI.Label(new Rect(rect.x + pad, rect.y + pad * 0.5f, innerW, textH), text, style);
    }

    // GameData.IsCompleted never gets reset when a new level starts (it's
    // in Matthew's Game Data.cs), so after finishing once it stays true
    // forever. Completion % does reset, so require both.
    private bool IsReallyComplete(GameData data)
    {
        return data.IsCompleted && data.Completion >= 1f;
    }

    // Shown once when the level's goal is reached: a dimmed screen with a
    // centered panel. "Keep Playing" hides it, "Main Menu" ends the level.
    private void DrawCompletePopup(GameData data)
    {
        if ( !IsReallyComplete(data) )
        {
            completeDismissed = false; // a new run can pop it up again
            return;
        }
        if (completeDismissed)
            return;

        // Dim the whole screen and block clicks from reaching the map.
        Rect screen = new Rect(0f, 0f, Screen.width, Screen.height);
        GUI.color = new Color(0.01f, 0.03f, 0.05f, 0.55f);
        GUI.DrawTexture(screen, Texture2D.whiteTexture);
        GUI.color = Color.white;
        UiTheme.RegisterBlocker(screen);

        string title = "LEVEL COMPLETE";
        string body  = data.Level.Name + "\n\n" +
                       "Time: " + data.TimeElapsed.ToString("F1") + "s\n\n" +
                       Header("GOAL") + "\n" + GoalToString();

        float pad    = UiTheme.S(24f);
        float gap    = UiTheme.S(12f);
        float btnH   = UiTheme.S(42f);
        float w      = Mathf.Min(Mathf.Clamp(Screen.width * 0.36f, UiTheme.S(380f), UiTheme.S(560f)), Screen.width * 0.9f);
        float innerW = w - pad * 2f;
        float titleH = popupTitleStyle.CalcHeight(new GUIContent(title), innerW);
        float bodyH  = popupBodyStyle .CalcHeight(new GUIContent(body ), innerW);
        float h      = pad + titleH + gap + bodyH + gap * 2f + btnH + pad;

        Rect rect = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
        UiTheme.DrawPanel(rect);

        float y = rect.y + pad;
        GUI.Label(new Rect(rect.x + pad, y, innerW, titleH), title, popupTitleStyle);
        y += titleH + gap;
        GUI.Label(new Rect(rect.x + pad, y, innerW, bodyH), body, popupBodyStyle);
        y += bodyH + gap * 2f;

        float btnW = (innerW - gap) * 0.5f;

        if ( UiTheme.Button(new Rect(rect.x + pad, y, btnW, btnH), "Keep Playing") )
            completeDismissed = true;

        if ( UiTheme.Button(new Rect(rect.x + pad + btnW + gap, y, btnW, btnH), "Main Menu") )
        {
            completeDismissed = false;
            data.LevelStop();
            GameMenu.SetPaused(false);
        }
    }

    private string Header(string text)
    {
        return "<color=#4DD9F2><b>" + text + "</b></color>";
    }

    private string BuildStatusText(GameData data)
    {
        string text = Header("RESOURCES") + "\n" + ItemAmountsToString(data.delivered);
        text += "\n" + Header("GOAL") + "\n" + GoalToString();
        text += "\n" + Header("AVAILABLE MACHINES") + "\n" + AvailableMachinesToString(data);
        text += $"\nTime: {data.TimeElapsed:F1}s   Completion: {data.Completion * 100f:F0}%";

        if ( IsReallyComplete(data) )
            text += "\n>>> LEVEL COMPLETE! <<<";

        return text;
    }

    // "Silicon: 3 / 10" for every item the level wants delivered, green
    // once that item's target is reached.
    private string GoalToString()
    {
        ItemRequirements requirements = ItemRequirements.INSTANCE;
        int count = requirements.Count();
        if (count == 0)
            return "  (none)";

        string text = "";
        for (int i = 0; i < count; i++)
        {
            ItemRequirement r     = requirements.Get(i);
            string          color = r.IsMet() ? "#5FD98A" : "#FFFFFF";
            text += $"  {r.Item.Name}: <color={color}>{r.Amount} / {r.Required}</color>\n";
        }
        return text.TrimEnd('\n');
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

        return any ? text.TrimEnd('\n') : "  (none, buy some in the shop)";
    }

    // Bottom-left panel: every ShopCollection entry, with its cost and a
    // Buy button, in a scrollable list. Spending directly manipulates
    // GameData (delivered, machinesAvailable) - both are public, no need
    // to touch Matthew's Machine Operations.cs for this.
    private void DrawShopPanel(GameData data)
    {
        int   entryCount = ShopCollection.Count();
        float pad        = UiTheme.S(14f);
        float margin     = UiTheme.S(10f);
        float rowH       = UiTheme.S(34f);
        float titleH     = UiTheme.S(30f);
        float w          = Mathf.Min(Mathf.Clamp(Screen.width * 0.32f, UiTheme.S(320f), UiTheme.S(480f)), Screen.width * 0.6f);

        float fullListH  = entryCount * rowH;
        float listH      = Mathf.Min(fullListH, Screen.height * 0.4f);
        bool  scrolls    = fullListH > listH;
        float h          = pad + titleH + listH + pad;

        Rect rect = new Rect(margin, Screen.height - h - margin, w, h);
        UiTheme.DrawPanel(rect);

        GUI.Label(new Rect(rect.x + pad, rect.y + pad * 0.7f, w - pad * 2f, titleH),
                  Header("SHOP") + "   (B to close)", shopTitleStyle);

        Rect view    = new Rect(rect.x + pad * 0.6f, rect.y + pad * 0.7f + titleH, w - pad * 1.2f, listH);
        Rect content = new Rect(0f, 0f, view.width - (scrolls ? UiTheme.S(18f) : 0f), fullListH);

        shopScroll = GUI.BeginScrollView(view, shopScroll, content);

        float buttonW = UiTheme.S(64f);

        for (byte i = 0; i < entryCount; i++)
        {
            ShopEntry entry = ShopCollection.Get(i);
            float     y     = i * rowH;

            string label = entry.machineType.Name + " - " + CostToString(entry.cost);
            GUI.Label(new Rect(UiTheme.S(6f), y, content.width - buttonW - UiTheme.S(14f), rowH), label, shopRowStyle);

            bool canAfford = data.delivered.IsMet(entry.cost);
            Rect buy       = new Rect(content.width - buttonW - UiTheme.S(4f), y + UiTheme.S(4f), buttonW, rowH - UiTheme.S(8f));

            if ( UiTheme.Button(buy, "Buy", canAfford) )
            {
                data.delivered.TrySubtract(entry.cost);
                data.machinesAvailable.Modify(entry.machineType, 1);
            }
        }

        GUI.EndScrollView();
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
