using UnityEngine;
using UnityEngine.UI;

public static class MenuStyle
{
    // Constants
    public static readonly Color BUTTON_COLOR_NORMAL      = Color.white;
    public static readonly Color BUTTON_COLOR_HIGHLIGHTED = new Color(0.65f, 0.80f, 1.00f);
    public static readonly Color BUTTON_COLOR_PRESSED     = new Color(1.00f, 0.55f, 0.55f);
    public static readonly Color BUTTON_COLOR_SELECTED    = new Color(0.55f, 0.85f, 0.65f);

    // Apply
    public static void Apply(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor      = BUTTON_COLOR_NORMAL;
        colors.highlightedColor = BUTTON_COLOR_HIGHLIGHTED;
        colors.pressedColor     = BUTTON_COLOR_PRESSED;
        colors.selectedColor    = BUTTON_COLOR_SELECTED;

        button.colors = colors;
    }
}
