using System;
using UnityEngine;

// Reusable Image For Drawing The Display Grid
public sealed class DisplayImage : IDisposable
{
    // Constants
    private static readonly Color32 BackgroundColor = new Color32(0, 0, 0, 255);

    // Member Variables
    private Texture2D texture;
    private Color32[] pixels;
    private int       width;
    private int       gridPixels;
    private int       gridX;
    private int       gridY;

    public Texture2D Texture => texture;

    // Begin Drawing
    public void Begin(MapView view)
    {
        int imageWidth  = (view.X1 - view.X0) * view.Pixels;
        int imageHeight = (view.Y1 - view.Y0) * view.Pixels;

        if (texture == null || texture.width != imageWidth || texture.height != imageHeight)
        {
            if (texture != null)
                UnityEngine.Object.Destroy(texture);

            texture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode   = TextureWrapMode.Clamp;
            texture.name       = "Node Factory Map";

            pixels = new Color32[imageWidth * imageHeight];
        }

        width      = imageWidth;
        gridPixels = view.Pixels;
        gridX      = view.X0;
        gridY      = view.Y0;

        Array.Fill(pixels, BackgroundColor);
    }

    // Draw Image
    public void Draw(DisplayGridImage image, int x, int y)
    {
        int pixelX = (x - gridX) * gridPixels;
        int pixelY = (y - gridY) * gridPixels;

        for (int iy = 0; iy < image.Length; iy++)
        {
            int source = iy * image.Length;
            int target = (texture.height - 1 - pixelY - iy) * width + pixelX;

            Array.Copy(image.pixels, source, pixels, target, image.Length);
        }
    }

    // Draw Image With Transparency
    public void Draw(DisplayGridImage image, int x, int y, float alpha)
    {
        int   pixelX  = (x  - gridX) * gridPixels;
        int   pixelY  = (y  - gridY) * gridPixels;
        float inverse = (1f - alpha);

        for (int iy = 0; iy < image.Length; iy++)
        {
            int source = iy * image.Length;
            int target = (texture.height - 1 - pixelY - iy) * width + pixelX;

            for (int ix = 0; ix < image.Length; ix++)
            {
                Color32 front = image.pixels[source + ix];
                Color32 back  =  this.pixels[target + ix];

                pixels[target + ix] = new Color32(
                    (byte)(back.r * inverse + front.r * alpha),
                    (byte)(back.g * inverse + front.g * alpha),
                    (byte)(back.b * inverse + front.b * alpha),
                    255
                );
            }
        }
    }

    public void Apply()
    {
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
    }

    public void Dispose()
    {
        if (texture != null)
            UnityEngine.Object.Destroy(texture);

        texture = null;
        pixels  = null;
    }
}