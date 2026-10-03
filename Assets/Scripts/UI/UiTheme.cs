using UnityEngine;

// Shared "dark aero" look - the same translucent glass panel, pulsing
// cyan glow, and left accent bar used by the main menu (Assets/Scripts/
// Menu/GameMenu.cs). Pulled out here so the HUD and the on-screen control
// hints can use the exact same panel style instead of their own separate
// flat black boxes, which is what they were before.
public static class UiTheme
{
    public static readonly Color ACCENT_COLOR = new Color(0.30f, 0.85f, 0.95f); // glowing cyan
    public static readonly Color PANEL_COLOR  = new Color(0.06f, 0.09f, 0.13f); // dark glass

    private static Texture2D panelTexture;
    private static Texture2D glowTexture;
    private static bool      built;

    // Glass panel + soft pulsing glow + left accent bar. Call only from
    // inside OnGUI (its textures are built lazily on first use, same as
    // GameMenu - Unity isn't ready to build GUI textures outside OnGUI).
    public static void DrawPanel(Rect box)
    {
        EnsureBuilt();

        float pulse = Mathf.Lerp(0.18f, 0.38f, (Mathf.Sin(Time.time * 1.4f) + 1f) * 0.5f);

        Rect glowRect = new Rect(box.x - 14f, box.y - 14f, box.width + 28f, box.height + 28f);
        GUI.color = new Color(ACCENT_COLOR.r, ACCENT_COLOR.g, ACCENT_COLOR.b, pulse);
        GUI.DrawTexture(glowRect, glowTexture);

        GUI.color = new Color(PANEL_COLOR.r, PANEL_COLOR.g, PANEL_COLOR.b, 0.72f); // translucent glass
        GUI.DrawTexture(box, panelTexture);

        GUI.color = ACCENT_COLOR;
        GUI.DrawTexture(new Rect(box.x, box.y + 6f, 3f, box.height - 12f), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    // Quick way to get a label style in the theme's palette without
    // building a GUIStyle by hand every time.
    public static GUIStyle MakeLabelStyle(int fontSize, FontStyle fontStyle, Color color, bool richText = false)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize  = fontSize;
        style.fontStyle = fontStyle;
        style.richText  = richText;
        style.wordWrap  = true;
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = color;
        return style;
    }

    private static void EnsureBuilt()
    {
        if (built)
            return;
        built = true;

        panelTexture = MakeRoundedGradientTexture(128, 0.10f, 0.015f, 0.85f, 1.15f);
        glowTexture  = MakeRoundedGradientTexture(128, 0.14f, 0.40f , 1f   , 1f   );
    }

    // Same rounded-rect-with-brightness-gradient generator as GameMenu's
    // panel texture (copied here so this class has no dependency on
    // GameMenu - it's the other way around, the HUD/hints depend on this).
    private static Texture2D MakeRoundedGradientTexture(int size, float cornerFraction, float blurFraction, float darkT, float brightT)
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

    private static float RoundedSquareDistance(float px, float py, float halfSize, float radius)
    {
        float qx = Mathf.Abs(px) - halfSize + radius;
        float qy = Mathf.Abs(py) - halfSize + radius;

        float outsideDist = Mathf.Sqrt(Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f)
                                      + Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f));
        float insideDist  = Mathf.Min(Mathf.Max(qx, qy), 0f);

        return outsideDist + insideDist - radius;
    }
}
