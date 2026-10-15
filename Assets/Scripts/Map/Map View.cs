using UnityEngine;

// Camera Position, Zoom, and Grid Image Dimensions
public class MapView
{
    // Constants
    public  const int   PIXELS_MIN     = 20;
    public  const int   PIXELS_MAX     = 60;
    public  const int   PIXELS_DEFAULT = 30;
    private const float ZOOM_SPEED     = 60f;

    // Member Variables
    private readonly MapSize mapSize = new MapSize();

    public Vector2 Center      { get; private set; }
    public float   PixelsFloat { get; private set; }
    public int     Pixels      { get; private set; }

    public int ImageX      { get; private set; }
    public int ImageY      { get; private set; }
    public int ImageWidth  { get; private set; }
    public int ImageHeight { get; private set; }

    public int X0 { get; private set; }
    public int Y0 { get; private set; }
    public int X1 { get; private set; }
    public int Y1 { get; private set; }

    // Constructor
    public MapView()
    {
        Clear();
    }

    public void Clear()
    {
        Set( MapSize.ZERO );
    }

    public void Set(MapSize other)
    {
        mapSize.Set(other);
        Center      = new Vector2(mapSize.Width * 0.5f, mapSize.Height * 0.5f);
        PixelsFloat = PIXELS_DEFAULT;
        UpdateView();
    }

    public void Scroll(Vector2 screenFraction)
    {
        Center += new Vector2(
            +screenFraction.x * Screen.width  / Pixels,
            -screenFraction.y * Screen.height / Pixels
        );
        UpdateView();
    }

    public void ChangePixelSize(float scale)
    {
        PixelsFloat = Mathf.Clamp(PixelsFloat * Mathf.Pow(scale, ZOOM_SPEED), PIXELS_MIN, PIXELS_MAX);
        UpdateView();
    }

    public void GameTick()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        Pixels = Mathf.RoundToInt(PixelsFloat);

        float halfX = Screen.width  / (2f * Pixels);
        float halfY = Screen.height / (2f * Pixels);

        Center = new Vector2(
            mapSize.Width <= halfX * 2f
                ? mapSize.Width * 0.5f
                : Mathf.Clamp(Center.x, halfX, mapSize.Width - halfX),

            mapSize.Height <= halfY * 2f
                ? mapSize.Height * 0.5f
                : Mathf.Clamp(Center.y, halfY, mapSize.Height - halfY)
        );

        int originX = Mathf.RoundToInt(Screen.width  * 0.5f - Center.x * Pixels);
        int originY = Mathf.RoundToInt(Screen.height * 0.5f - Center.y * Pixels);

        X0 = Mathf.FloorToInt(-originX / (float)Pixels);
        Y0 = Mathf.FloorToInt(-originY / (float)Pixels);

        ImageX = originX + X0 * Pixels;
        ImageY = originY + Y0 * Pixels;

        int columns = Mathf.CeilToInt((Screen.width  - ImageX) / (float)Pixels);
        int rows    = Mathf.CeilToInt((Screen.height - ImageY) / (float)Pixels);

        X1 = X0 + columns;
        Y1 = Y0 + rows;

        ImageWidth  = columns * Pixels;
        ImageHeight =    rows * Pixels;
    }

    public Vector2Int ScreenToGridInt(Vector2 position)
    {
        return new Vector2Int(
            X0 + Mathf.FloorToInt((position.x - ImageX) / Pixels),
            Y0 + Mathf.FloorToInt((position.y - ImageY) / Pixels)
        );
    }

    public void Save(IOWriter writer)
    {
        mapSize.Save(writer);
        writer.WriteVector2(Center     );
        writer.WriteFloat  (PixelsFloat);
    }

    public void Load(IOReader reader)
    {
        mapSize.Load(reader);
        Center      = reader.ReadVector2();
        PixelsFloat = reader.ReadFloat  ();
        UpdateView();
    }

    public void Debug(int indentation)
    {
        DebugFile.Log("MapView", indentation);
        indentation++;

        mapSize.Debug(indentation);
        DebugFile.Log("Center:      " + Center      , indentation);
        DebugFile.Log("PixelsFloat: " + PixelsFloat , indentation);
        DebugFile.Log("Pixels:      " + Pixels      , indentation);
        DebugFile.Log("ImageX:      " + ImageX      , indentation);
        DebugFile.Log("ImageY:      " + ImageY      , indentation);
        DebugFile.Log("ImageWidth:  " + ImageWidth  , indentation);
        DebugFile.Log("ImageHeight: " + ImageHeight , indentation);
        DebugFile.Log("X0:          " + X0          , indentation);
        DebugFile.Log("Y0:          " + Y0          , indentation);
        DebugFile.Log("X1:          " + X1          , indentation);
        DebugFile.Log("Y1:          " + Y1          , indentation);
    }
}