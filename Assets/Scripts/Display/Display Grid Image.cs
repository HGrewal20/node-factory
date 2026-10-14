using UnityEngine;

// Reusable Image Class For Drawing Machines
// Performance Is Not a Concern Because they are Small, Buffered, and Made Only When Needed
// Alpha is ignored, but there is an option to draw with transparency later
public sealed class DisplayGridImage
{
    // Constants
    private static readonly Color32 DEFAULT_COLOR = new Color32(0, 0, 0, 0);

    // Member Variables
    public readonly Color32[] pixels;
    public readonly int       Length;

    // Constructor
    public DisplayGridImage(int length)
    {
        Length = length;
        pixels = new Color32[length * length];
    }

    public void Clear()                 { Clear(DEFAULT_COLOR); }
    public void Clear(Color32 color)
    {
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
    }

    public void SetPixel(Color32 color, int x, int y)
    {
        pixels[y * Length + x] = color;
    }

    public void Fill(Color32 color, int x0, int y0, int x1, int y1)
    {
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++)
                SetPixel(color, x, y);
    }

    public void LineX(Color32 color, int y, int x0, int x1) { for (int x = x0; x <= x1; x++) SetPixel(color, x, y); }
    public void LineY(Color32 color, int x, int y0, int y1) { for (int y = y0; y <= y1; y++) SetPixel(color, x, y); }

    public void Rect(Color32 color, int x0, int y0, int x1, int y1)
    {
        LineX(color, y0, x0, x1);
        LineX(color, y1, x0, x1);
        LineY(color, x0, y0, y1);
        LineY(color, x1, y0, y1);
    }

    public DisplayGridImage Rotate90CW()
    {
        DisplayGridImage rotated = new DisplayGridImage(Length);

        for (int y = 0; y < Length; y++)
            for (int x = 0; x < Length; x++)
            {
                // int newX = Length - 1 - y;
                // int newY = x;

                int newX = y;
                int newY = Length - 1 - x;

                rotated.SetPixel(pixels[y * Length + x], newX, newY);
            }

        return rotated;
    }
}
