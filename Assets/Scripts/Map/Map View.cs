using UnityEngine;

public class MapView
{
    // Static Variables
    public const float ZOOM_MIN             = 0.5f;
    public const float ZOOM_MAX             = 4.0f;
    public const float BASE_VISIBLE_HEIGHT  = 40.0f;

    // Member Variables
    private MapSize mapSize         = new MapSize();
    public  Vector2 Center           { get; private set; }
    public  float   Zoom             { get; private set; }  // Higher zoom = more zoomed in

    // Visible size in grid units.
    public  float   VisibleWidth     { get; private set; }
    public  float   VisibleHeight    { get; private set; }

    // Visible grid cells. Min and max are inclusive.
    public  int     XMin             { get; private set; }  // Left
    public  int     XMax             { get; private set; }  // Right
    public  int     YMin             { get; private set; }  // Bot
    public  int     YMax             { get; private set; }  // Top

    // Constructor
    public MapView()
    {
        Clear();
    }

    // Clear
    public void Clear()
    {
        mapSize       .Clear();
        SetCenter     (Vector2.zero);
        SetZoom       (1.0f        );

        VisibleWidth  = 0f;
        VisibleHeight = 0f;

        XMin          = 0;
        XMax          = 0;
        YMin          = 0;
        YMax          = 0;
    }

    // Map Size
    public void Set(MapSize other)
    {
        Clear    ();
        mapSize  .Set(other);
        SetCenter();
    }

    // Center
    public void SetCenter()                 { SetCenter(mapSize.Width / 2f, mapSize.Height / 2f); }
    public void SetCenter(Vector2 center)   { SetCenter(center.x          , center.y           ); }
    public void SetCenter(float x, float y)
    {
        Center = new Vector2(
            Mathf.Clamp(x, 0f, mapSize.Width ),
            Mathf.Clamp(y, 0f, mapSize.Height)
        );
    }

    public void Scroll(Vector2 delta)
    {
        delta /= Zoom;
        SetCenter(Center + delta);
    }

    // Zoom
    public void SetZoom(float zoom)
    {
        Zoom = Mathf.Clamp(zoom, ZOOM_MIN, ZOOM_MAX);
    }

    public void ZoomScale(float scale)
    {
        SetZoom(Zoom * scale);
    }

    public void ZoomChange(float delta)
    {
        if (delta > 0)      ZoomScale(     (1f + delta) / 1f);
        else                ZoomScale(1f / (1f - delta)     );
    }

    // Game Tick
    public void GameTick()
    {
        // Determine how much of the grid fits on screen.
        float aspect = (Screen.height > 0) ? (float)Screen.width / Screen.height : 1.0f;

        VisibleHeight = (BASE_VISIBLE_HEIGHT / Zoom  );
        VisibleWidth  = (VisibleHeight       * aspect);

        // Determine exact visible edges.
        float halfWidth  = (VisibleWidth  * 0.5f);
        float halfHeight = (VisibleHeight * 0.5f);

        float left       = (Center.x - halfWidth );
        float right      = (Center.x + halfWidth );
        float bottom     = (Center.y - halfHeight);
        float top        = (Center.y + halfHeight);

        // Convert visible area to grid cells.
        XMin = (int) left;
        XMax = (int) right;

        YMin = (int) bottom;
        YMax = (int) top;

        // Clamp to map.
        XMin = Mathf.Clamp(XMin, 0, mapSize.Width  - 1);
        XMax = Mathf.Clamp(XMax, 0, mapSize.Width  - 1);

        YMin = Mathf.Clamp(YMin, 0, mapSize.Height - 1);
        YMax = Mathf.Clamp(YMax, 0, mapSize.Height - 1);
    }

    // Conversion
    public Vector2 ScreenToGrid(Vector2 screenPosition)
    {
        float xPercent = screenPosition.x / Screen.width;
        float yPercent = screenPosition.y / Screen.height;

        float x = Center.x + ((xPercent - 0.5f) * VisibleWidth );
        float y = Center.y + ((yPercent - 0.5f) * VisibleHeight);

        return new Vector2(x, y);
    }

    public Vector2Int ScreenToGridInt(Vector2 screenPosition)
    {
        Vector2 gridPosition = ScreenToGrid(screenPosition);

        int x = (int) gridPosition.x;
        int y = (int) gridPosition.y;

        return new Vector2Int(x, y);
    }

    public Rect ScreenToGrid(Rect screenRect)
    {
        Vector2 min = ScreenToGrid(screenRect.min);
        Vector2 max = ScreenToGrid(screenRect.max);

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    public RectInt ScreenToGridInt(Rect screenRect)
    {
        Vector2 min = ScreenToGrid(screenRect.min);
        Vector2 max = ScreenToGrid(screenRect.max);

        int xMin = (int) min.x;
        int xMax = (int) max.x;

        int yMin = (int) min.y;
        int yMax = (int) max.y;

        return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    public Vector2 GridToScreen(Vector2     gridPosition) { return GridToScreen(gridPosition.x, gridPosition.y); }
    public Vector2 GridToScreen(int   gridX, int   gridY) { return GridToScreen(gridX         , gridY         ); }
    public Vector2 GridToScreen(float gridX, float gridY)
    {
        float x = ((gridX - Center.x) / VisibleWidth ) + 0.5f;
        float y = ((gridY - Center.y) / VisibleHeight) + 0.5f;

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
        mapSize.Save        (writer       );
        writer .WriteVector2(Center       );
        writer .WriteFloat  (Zoom         );

        writer.WriteFloat   (VisibleWidth );
        writer.WriteFloat   (VisibleHeight);

        writer.WriteInt     (XMin         );
        writer.WriteInt     (XMax         );
        writer.WriteInt     (YMin         );
        writer.WriteInt     (YMax         );
    }

    public void Load(IOReader reader)
    {
        mapSize       .Load(reader);
        Center        = reader.ReadVector2();
        Zoom          = reader.ReadFloat  ();

        VisibleWidth  = reader.ReadFloat  ();
        VisibleHeight = reader.ReadFloat  ();

        XMin          = reader.ReadInt    ();
        XMax          = reader.ReadInt    ();
        YMin          = reader.ReadInt    ();
        YMax          = reader.ReadInt    ();
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MapView", indentation);
        indentation++;

        mapSize.Debug(indentation);
        DebugFile.Log("Center:        " + Center       , indentation);
        DebugFile.Log("Zoom:          " + Zoom         , indentation);

        DebugFile.Log("VisibleWidth:  " + VisibleWidth , indentation);
        DebugFile.Log("VisibleHeight: " + VisibleHeight, indentation);

        DebugFile.Log("XMin:          " + XMin         , indentation);
        DebugFile.Log("XMax:          " + XMax         , indentation);
        DebugFile.Log("YMin:          " + YMin         , indentation);
        DebugFile.Log("YMax:          " + YMax         , indentation);
    }
}
