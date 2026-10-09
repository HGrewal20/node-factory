using UnityEngine;

public class MapView
{
    // Constants
    public const int   PIXELS_MIN     =  8;
    public const int   PIXELS_MAX     = 40;
    public const int   PIXELS_DEFAULT = 20;

    public const float ZOOM_MIN       = 1.0f;
    public const float ZOOM_MAX       = PIXELS_MAX     / (float) PIXELS_MIN;
    public const float ZOOM_DEFAULT   = PIXELS_DEFAULT / (float) PIXELS_MIN;

    // Member Variables
    private readonly MapSize mapSize = new MapSize();

    public Vector2 Center        { get; private set; }
    public float   Zoom          { get; private set; }

    // Visible size in grid cells.
    public int     VisibleWidth  { get; private set; }
    public int     VisibleHeight { get; private set; }

    // Visible grid cells. Min and max are inclusive.
    public int     XMin          { get; private set; }
    public int     XMax          { get; private set; }
    public int     YMin          { get; private set; }
    public int     YMax          { get; private set; }

    // Constructor
    public MapView()
    {
        Clear();
    }

    // Clear
    public void Clear()
    {
        mapSize.Clear();

        Center = Vector2.zero;
        Zoom   = ZOOM_DEFAULT;

        UpdateView();
    }

    // Map Size
    public void Set(MapSize other)
    {
        mapSize.Set(other);

        Center = new Vector2(mapSize.Width * 0.5f, mapSize.Height * 0.5f);
        Zoom   = ZOOM_DEFAULT;

        UpdateView();
    }

    // Center
    public void SetCenter()               { SetCenter(mapSize.Width * 0.5f, mapSize.Height * 0.5f); }
    public void SetCenter(Vector2 center) { SetCenter(center.x, center.y); }

    public void SetCenter(float x, float y)
    {
        Center = new Vector2(x, y);
        UpdateView();
    }

    public void Scroll(Vector2 delta)
    {
        SetCenter(Center + delta / Zoom);
    }

    // Zoom
    public void SetZoom(float zoom)
    {
        Zoom = Mathf.Clamp(zoom, ZOOM_MIN, ZOOM_MAX);
        UpdateView();
    }

    public void ZoomScale(float scale)
    {
        SetZoom(Zoom * scale);
    }

    public void ZoomChange(float delta)
    {
        if (delta > 0f) ZoomScale( (1f + delta) /  1f           );
        else            ZoomScale(           1f / (1f - delta)  );
    }

    // Game Tick
    public void GameTick()
    {
        UpdateView();
    }

    // Update View
    private void UpdateView()
    {
        int pixels = Mathf.RoundToInt(PIXELS_MIN * Zoom);

        VisibleWidth  = Mathf.Max(1, Mathf.RoundToInt(Screen.width  / pixels));
        VisibleHeight = Mathf.Max(1, Mathf.RoundToInt(Screen.height / pixels));

        // Clamp center while accounting for the visible area.
        float halfWidth  = VisibleWidth  * 0.5f;
        float halfHeight = VisibleHeight * 0.5f;

        float x = (mapSize.Width <= VisibleWidth)
            ? mapSize.Width * 0.5f
            : Mathf.Clamp(Center.x, halfWidth, mapSize.Width - halfWidth);

        float y = (mapSize.Height <= VisibleHeight)
            ? mapSize.Height * 0.5f
            : Mathf.Clamp(Center.y, halfHeight, mapSize.Height - halfHeight);

        Center = new Vector2(x, y);

        // Determine visible cell bounds.
        float left   = Center.x - halfWidth;
        float right  = Center.x + halfWidth;
        float bottom = Center.y - halfHeight;
        float top    = Center.y + halfHeight;

        XMin = Mathf.Max(0, Mathf.FloorToInt(left));
        XMax = Mathf.Min(mapSize.Width - 1, Mathf.CeilToInt(right) - 1);

        YMin = Mathf.Max(0, Mathf.FloorToInt(bottom));
        YMax = Mathf.Min(mapSize.Height - 1, Mathf.CeilToInt(top) - 1);
    }

    // Conversion
    public Vector2 ScreenToGrid(Vector2 screenPosition)
    {
        float width  = Mathf.Max(1, Screen.width );
        float height = Mathf.Max(1, Screen.height);

        float xPercent = screenPosition.x / width;
        float yPercent = screenPosition.y / height;

        float x = Center.x + (xPercent - 0.5f) * VisibleWidth;
        float y = Center.y + (yPercent - 0.5f) * VisibleHeight;

        return new Vector2(x, y);
    }

    public Vector2Int ScreenToGridInt(Vector2 screenPosition)
    {
        Vector2 position = ScreenToGrid(screenPosition);

        return new Vector2Int(
            Mathf.FloorToInt(position.x),
            Mathf.FloorToInt(position.y)
        );
    }

    public Rect ScreenToGrid(Rect screenRect)
    {
        Vector2 min = ScreenToGrid(screenRect.min);
        Vector2 max = ScreenToGrid(screenRect.max);

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    public RectInt ScreenToGridInt(Rect screenRect)
    {
        Rect rect = ScreenToGrid(screenRect);

        int xMin = Mathf.FloorToInt(rect.xMin);
        int xMax = Mathf.CeilToInt (rect.xMax);
        int yMin = Mathf.FloorToInt(rect.yMin);
        int yMax = Mathf.CeilToInt (rect.yMax);

        return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    public Vector2 GridToScreen(Vector2 gridPosition)     { return GridToScreen(gridPosition.x, gridPosition.y); }
    public Vector2 GridToScreen(int gridX, int gridY)     { return GridToScreen((float)gridX, (float)gridY); }

    public Vector2 GridToScreen(float gridX, float gridY)
    {
        float x = (gridX - Center.x) / VisibleWidth + 0.5f;
        float y = (gridY - Center.y) / VisibleHeight + 0.5f;

        return new Vector2(x * Screen.width, y * Screen.height);
    }

    public Rect GridToScreen(Rect gridRect)
    {
        Vector2 min = GridToScreen(gridRect.min);
        Vector2 max = GridToScreen(gridRect.max);

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    // IO
    public void Save(IOWriter writer)
    {
        mapSize.Save(writer);
        writer.WriteVector2 (Center         );
        writer.WriteFloat   (Zoom           );

        // Retained for compatibility with existing save files.
        writer.WriteFloat   (VisibleWidth   );
        writer.WriteFloat   (VisibleHeight  );

        writer.WriteInt     (XMin           );
        writer.WriteInt     (XMax           );
        writer.WriteInt     (YMin           );
        writer.WriteInt     (YMax           );
    }

    public void Load(IOReader reader)
    {
        mapSize.Load(reader);

        Center = reader.ReadVector2();
        Zoom  = Mathf.Clamp(reader.ReadFloat(), ZOOM_MIN, ZOOM_MAX);

        // Read existing format, then rebuild derived values.
        reader.ReadFloat();
        reader.ReadFloat();

        reader.ReadInt();
        reader.ReadInt();
        reader.ReadInt();
        reader.ReadInt();

        UpdateView();
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MapView", indentation);
        indentation++;

        mapSize.Debug(indentation);
        DebugFile.Log("Center:        " + Center        , indentation);
        DebugFile.Log("Zoom:          " + Zoom          , indentation);

        DebugFile.Log("VisibleWidth:  " + VisibleWidth  , indentation);
        DebugFile.Log("VisibleHeight: " + VisibleHeight , indentation);

        DebugFile.Log("XMin:          " + XMin          , indentation);
        DebugFile.Log("XMax:          " + XMax          , indentation);
        DebugFile.Log("YMin:          " + YMin          , indentation);
        DebugFile.Log("YMax:          " + YMax          , indentation);
    }
}