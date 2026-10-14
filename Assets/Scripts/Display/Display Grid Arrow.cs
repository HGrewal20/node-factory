using UnityEngine;

// Helper For Drawing Pixel Arrows
public static class DisplayGridArrow
{
    // Draws An Upward Arrow
    public static void Draw(DisplayGridImage image, Color32 color)
    {
        int size  = image.Length;
        int inset = Mathf.Max(1, size / 8);

        // Arrowhead: 45 Degree Slopes
        int headHeight = Mathf.Max(2, (size - inset * 2) / 2);
        int tipY       = size - inset - 1;
        int centerX    = size / 2;

        // Tail
        int tailHalfWidth = Mathf.Max(1, headHeight / 3);
        int headBottom    = tipY - headHeight + 1;

        image.Fill(
            color,
            centerX - tailHalfWidth,
            inset,
            centerX + tailHalfWidth + 1,
            headBottom + 1
        );

        // Arrowhead
        for (int i = 0; i < headHeight; i++)
        {
            int y = tipY - i;
            image.LineX(color, y, centerX - i, centerX + i);
        }
    }
}