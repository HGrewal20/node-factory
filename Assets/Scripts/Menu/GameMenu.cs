using UnityEngine;

// The main menu and pause menu.
// Nothing starts the level on its own anymore - you have to press
// "Start Game" here first (GameDisplay used to do that automatically,
// that line got removed). Escape opens/closes the pause menu once a
// level is running.
//
// Visual style: "dark aero" - a translucent glass sidebar anchored to
// the left edge (scales with the window instead of a fixed pixel size),
// a glowing cyan accent that gently pulses, and glossy top-lit buttons
// with a hover accent bar. All generated procedurally in code, no image
// files, same trick as the earlier machine-shading experiment.
public class GameMenu : MonoBehaviour
{
    // GameDisplay and GameInteraction check this so they know to stop
    // drawing/responding to clicks while the pause menu is open.
    public static bool IsPaused { get; private set; }

    private static readonly Color ACCENT_COLOR = new Color(0.30f, 0.85f, 0.95f); // glowing cyan
    private static readonly Color PANEL_COLOR  = new Color(0.06f, 0.09f, 0.13f); // dark glass
    private static readonly Color BUTTON_COLOR = new Color(0.14f, 0.22f, 0.30f); // lighter glass

    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private Texture2D glowTexture;
    private GUIStyle  titleStyle;
    private GUIStyle  subtitleStyle;
    private GUIStyle  buttonStyle;
    private GUIStyle  changelogHeaderStyle;
    private GUIStyle  changelogVersionStyle;
    private GUIStyle  changelogSectionStyle;
    private GUIStyle  changelogEntryStyle;
    private Vector2   changelogScroll;
    private bool      built;

    // Player-facing changelog, organized like a real release note - not
    // the full internal dev changelog (Assets/Change Log.cs, team notes).
    // One entry per version; each lists what was Added / Modified /
    // Removed from a player's point of view.
    private class ChangelogVersion
    {
        public string   Version;
        public string[] Added;
        public string[] Modified;
        public string[] Removed;
    }

    private static readonly ChangelogVersion[] CHANGELOG = new ChangelogVersion[]
    {
        new ChangelogVersion
        {
            Version = GameConstants.VERSION_TEXT,
            Added = new string[]
            {
                "Main menu and pause menu",
                "Grid, machine, and node rendering",
                "Place, connect, and remove machines with the mouse",
                "Pan (WASD/arrows) and zoom (scroll wheel)",
                "HUD: resources, available machines, and progress",
                "Shop for buying new machines",
            },
            Modified = new string[]
            {
                "Machine and node visuals (colors, shapes)",
                "Input handling, moved to the new Input System",
                "Control hints and resource panel now match the menu's look",
            },
            Removed = new string[]
            {
                "Temporary debug HUD overlay (replaced by the real HUD)",
            },
        },
    };

    void OnGUI()
    {
        GameData data = GameData.INSTANCE;
        EnsureBuilt();

        if ( !data.IsPlaying() )
        {
            IsPaused = false; // no such thing as "paused" when not even playing
            DrawMainMenu(data);
            return;
        }

        HandleEscape();

        if (IsPaused)
            DrawPauseMenu(data);
    }

    private void HandleEscape()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            IsPaused = !IsPaused;
            e.Use();
        }
    }

    private void DrawMainMenu(GameData data)
    {
        DrawDimBackground();

        Rect box = LeftAnchoredBox(300f);
        DrawPanel(box);

        GUILayout.BeginArea(Inset(box));
        GUILayout.Space(10);
        GUILayout.Label("NODE FACTORY", titleStyle);
        GUILayout.Label("AUTOMATION  ·  v" + GameConstants.VERSION_TEXT, subtitleStyle);
        GUILayout.Space(26);

        if ( GlossyButton("Start Game") )
            data.LevelStart( LevelCollection.Get(0) );

        GUILayout.Space(8);

        if ( GlossyButton("Quit") )
            Application.Quit();

        GUILayout.EndArea();

        DrawChangelogPanel();
    }

    private void DrawPauseMenu(GameData data)
    {
        DrawDimBackground();

        Rect box = LeftAnchoredBox(320f);
        DrawPanel(box);

        GUILayout.BeginArea(Inset(box));
        GUILayout.Space(10);
        GUILayout.Label("PAUSED", titleStyle);
        GUILayout.Label("GAME IS SUSPENDED", subtitleStyle);
        GUILayout.Space(26);

        if ( GlossyButton("Resume") )
            IsPaused = false;

        GUILayout.Space(8);

        if ( GlossyButton("Exit to Main Menu") )
        {
            data.LevelStop();
            IsPaused = false;
        }

        GUILayout.Space(8);

        if ( GlossyButton("Quit Game") )
            Application.Quit();

        GUILayout.EndArea();
    }

    // Anchored to the left edge, vertically centered, sized as a fraction
    // of the window instead of a fixed pixel box - stays in proportion
    // whether the window is maximized, small, or ultrawide.
    private Rect LeftAnchoredBox(float height)
    {
        float marginX = Mathf.Max(Screen.width  * 0.05f, 24f);
        float width   = Mathf.Clamp(Screen.width * 0.24f, 260f, 420f);
        float y       = (Screen.height - height) / 2f;

        return new Rect(marginX, y, width, height);
    }

    private Rect Inset(Rect box)
    {
        return new Rect(box.x + 18f, box.y, box.width - 36f, box.height);
    }

    // Mirrors LeftAnchoredBox, but anchored to the right edge instead -
    // same responsive margin/width logic.
    private Rect RightAnchoredBox(float height, float leftBoxWidth)
    {
        float marginX = Mathf.Max(Screen.width  * 0.05f, 24f);
        float width   = Mathf.Clamp(Screen.width * 0.26f, 280f, 460f);
        float y       = (Screen.height - height) / 2f;
        float x       = Screen.width - marginX - width;

        return new Rect(x, y, width, height);
    }

    // A second glass panel to the right of the main menu, listing what's
    // changed so far, organized by version and Added/Modified/Removed -
    // like a real release's patch notes.
    private void DrawChangelogPanel()
    {
        // Own height, capped so it never runs off-screen - independent of
        // the start box, so a long changelog doesn't make that box taller.
        float height = Mathf.Min(Screen.height * 0.75f, 560f);
        Rect  box    = RightAnchoredBox(height, 0f);
        DrawPanel(box);

        GUILayout.BeginArea(Inset(box));
        GUILayout.Space(10);
        GUILayout.Label("CHANGELOG", changelogHeaderStyle);
        GUILayout.Space(12);

        changelogScroll = GUILayout.BeginScrollView(
            changelogScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        foreach (ChangelogVersion version in CHANGELOG)
            DrawChangelogVersion(version);

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void DrawChangelogVersion(ChangelogVersion version)
    {
        GUILayout.Label("v" + version.Version, changelogVersionStyle);
        GUILayout.Space(6);

        DrawChangelogSection("ADDED"   , version.Added   , "#5FD98A"); // green
        DrawChangelogSection("MODIFIED", version.Modified, "#F2C14D"); // amber
        DrawChangelogSection("REMOVED" , version.Removed , "#E0665C"); // red
    }

    private void DrawChangelogSection(string label, string[] entries, string bulletHex)
    {
        if (entries == null || entries.Length == 0)
            return;

        GUILayout.Label(label, changelogSectionStyle);

        foreach (string entry in entries)
            GUILayout.Label("<color=" + bulletHex + ">•</color>  " + entry, changelogEntryStyle);

        GUILayout.Space(8);
    }

    // Darkens whatever's behind the menu (the grid/HUD). Slightly blue
    // tinted instead of pure black, and kept fairly light since the panel
    // itself is translucent too - the layering is the point.
    private void DrawDimBackground()
    {
        GUI.color = new Color(0.01f, 0.03f, 0.05f, 0.45f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    // A soft, gently pulsing glow behind the panel, the translucent glass
    // panel itself, then a bright accent strip down the left edge.
    private void DrawPanel(Rect box)
    {
        float pulse = Mathf.Lerp(0.18f, 0.38f, (Mathf.Sin(Time.time * 1.4f) + 1f) * 0.5f);

        Rect glowRect = new Rect(box.x - 14f, box.y - 14f, box.width + 28f, box.height + 28f);
        GUI.color = new Color(ACCENT_COLOR.r, ACCENT_COLOR.g, ACCENT_COLOR.b, pulse);
        GUI.DrawTexture(glowRect, glowTexture);

        GUI.color = new Color(PANEL_COLOR.r, PANEL_COLOR.g, PANEL_COLOR.b, 0.68f); // translucent glass
        GUI.DrawTexture(box, panelTexture);

        GUI.color = ACCENT_COLOR;
        GUI.DrawTexture(new Rect(box.x, box.y + 6f, 3f, box.height - 12f), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    // Draws a glossy top-lit chip, a translucent highlight band across the
    // top, a left accent bar when hovered, then an invisible real
    // GUI.Button on top for the actual click + left-aligned label text.
    private bool GlossyButton(string label)
    {
        Rect rect = GUILayoutUtility.GetRect(10f, 38f, GUILayout.ExpandWidth(true));
        bool hover = rect.Contains(Event.current.mousePosition);

        GUI.color = BUTTON_COLOR;
        GUI.DrawTexture(rect, buttonTexture);

        Rect highlight = new Rect(rect.x + 3f, rect.y + 2f, rect.width - 6f, rect.height * 0.45f);
        GUI.color = new Color(1f, 1f, 1f, 0.14f);
        GUI.DrawTexture(highlight, buttonTexture);

        if (hover)
        {
            GUI.color = ACCENT_COLOR;
            GUI.DrawTexture(new Rect(rect.x, rect.y + 3f, 3f, rect.height - 6f), Texture2D.whiteTexture);
        }

        GUI.color = Color.white;
        bool clicked = GUI.Button(rect, label, buttonStyle);

        return clicked;
    }

    // GUIStyle/Texture2D objects can't be built outside OnGUI (Unity isn't
    // ready for them yet), so build everything once on first use.
    private void EnsureBuilt()
    {
        if (built)
            return;
        built = true;

        panelTexture  = MakeRoundedGradientTexture(128, 0.10f, 0.015f, 0.85f, 1.15f);
        buttonTexture = MakeRoundedGradientTexture( 64, 0.22f, 0.02f , 0.80f, 1.35f);
        glowTexture   = MakeRoundedGradientTexture(128, 0.14f, 0.40f , 1f   , 1f   );

        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize   = 26;
        titleStyle.fontStyle  = FontStyle.BoldAndItalic;
        titleStyle.alignment  = TextAnchor.MiddleLeft;
        titleStyle.normal.textColor = Color.white;

        subtitleStyle = new GUIStyle(GUI.skin.label);
        subtitleStyle.fontSize  = 12;
        subtitleStyle.fontStyle = FontStyle.Italic;
        subtitleStyle.alignment = TextAnchor.MiddleLeft;
        subtitleStyle.normal.textColor = new Color(0.65f, 0.80f, 0.85f);

        Texture2D clear = new Texture2D(1, 1);
        clear.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
        clear.Apply();

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize   = 15;
        buttonStyle.fontStyle  = FontStyle.Bold;
        buttonStyle.alignment  = TextAnchor.MiddleLeft;
        buttonStyle.padding.left = 16;
        buttonStyle.normal.background = clear;
        buttonStyle.hover .background = clear;
        buttonStyle.active.background = clear;
        buttonStyle.normal.textColor  = Color.white;
        buttonStyle.hover .textColor  = ACCENT_COLOR;
        buttonStyle.active.textColor  = ACCENT_COLOR;

        changelogHeaderStyle = new GUIStyle(GUI.skin.label);
        changelogHeaderStyle.fontSize   = 15;
        changelogHeaderStyle.fontStyle  = FontStyle.Bold;
        changelogHeaderStyle.alignment  = TextAnchor.MiddleLeft;
        changelogHeaderStyle.normal.textColor = Color.white;

        changelogEntryStyle = new GUIStyle(GUI.skin.label);
        changelogEntryStyle.fontSize   = 13;
        changelogEntryStyle.wordWrap   = true;
        changelogEntryStyle.richText   = true;
        changelogEntryStyle.alignment  = TextAnchor.UpperLeft;
        changelogEntryStyle.margin.bottom = 6;
        changelogEntryStyle.normal.textColor = new Color(0.85f, 0.90f, 0.92f);

        changelogVersionStyle = new GUIStyle(GUI.skin.label);
        changelogVersionStyle.fontSize   = 14;
        changelogVersionStyle.fontStyle  = FontStyle.Bold;
        changelogVersionStyle.alignment  = TextAnchor.MiddleLeft;
        changelogVersionStyle.margin.bottom = 4;
        changelogVersionStyle.normal.textColor = ACCENT_COLOR;

        changelogSectionStyle = new GUIStyle(GUI.skin.label);
        changelogSectionStyle.fontSize   = 11;
        changelogSectionStyle.fontStyle  = FontStyle.Bold;
        changelogSectionStyle.alignment  = TextAnchor.MiddleLeft;
        changelogSectionStyle.margin.top    = 2;
        changelogSectionStyle.margin.bottom = 2;
        changelogSectionStyle.normal.textColor = new Color(0.65f, 0.70f, 0.75f);
    }

    // Rounded rect with a vertical brightness gradient baked into RGB
    // (brighter near the top, like light catching glass). blurFraction
    // kept small for crisp edges on panels/buttons, large for the soft
    // glow texture.
    private Texture2D MakeRoundedGradientTexture(int size, float cornerFraction, float blurFraction, float darkT, float brightT)
    {
        Texture2D tex = new Texture2D(size, size);
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        float half   = size * 0.5f;
        float radius = half * cornerFraction;
        float blur   = half * blurFraction;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float px = (x + 0.5f) - half;
                float py = (y + 0.5f) - half;

                float dist  = RoundedSquareDistance(px, py, half - blur, radius);
                float alpha = 1f - Mathf.Clamp01((dist + blur) / (blur * 2f));

                float verticalT  = (float) y / (size - 1); // 0 bottom, 1 top
                float brightness = Mathf.Lerp(darkT, brightT, verticalT);

                tex.SetPixel(x, y, new Color(brightness, brightness, brightness, alpha));
            }
        }

        tex.Apply();
        return tex;
    }

    // Signed distance from (px, py) to a square of half-size "halfSize"
    // with rounded corners of radius "radius". Negative = inside the shape.
    private float RoundedSquareDistance(float px, float py, float halfSize, float radius)
    {
        float qx = Mathf.Abs(px) - halfSize + radius;
        float qy = Mathf.Abs(py) - halfSize + radius;

        float outsideDist = Mathf.Sqrt(Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f)
                                      + Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f));
        float insideDist  = Mathf.Min(Mathf.Max(qx, qy), 0f);

        return outsideDist + insideDist - radius;
    }
}
