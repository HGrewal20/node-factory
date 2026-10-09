using UnityEngine;

public class MapSize
{
    // Static Variables
    public const int VALUE_MIN =  15;
    public const int VALUE_MAX = 200;

    // Member Variables
    public int Width  { get; private set; }
    public int Height { get; private set; }

    // Constructor
    public MapSize()                        {}
    public MapSize(int width, int height)   { Set(width, height); }

    // Clear
    public void Clear()
    {
        Width  = 0;
        Height = 0;
    }

    // Set
    public void Set(MapSize other)
    {
        Width  = other.Width;
        Height = other.Height;
    }
    public void Set(int width, int height)
    {
        Width  = SizeClamp(width );
        Height = SizeClamp(height);
    }
    private static int SizeClamp(int value)
    {
        value = Mathf.Max(value, VALUE_MIN);
        value = Mathf.Min(value, VALUE_MAX);
        return value;
    }

    // Clamp
    public int ClampGridX(int value) => Mathf.Clamp(value, 0, Width  - 1);
    public int ClampGridY(int value) => Mathf.Clamp(value, 0, Height - 1);

    public Vector2Int ClampGrid(Vector2Int loc)
    {
        return new Vector2Int( ClampGridX(loc.x), ClampGridY(loc.y) );
    }

    public RectInt ClampGrid(RectInt rect)
    {
        int xMin = Mathf.Clamp(rect.xMin, 0, Width );
        int yMin = Mathf.Clamp(rect.yMin, 0, Height);
        int xMax = Mathf.Clamp(rect.xMax, 0, Width );
        int yMax = Mathf.Clamp(rect.yMax, 0, Height);

        return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    // IO
    public void Save(IOWriter writer)
    {
        writer.WriteInt(Width );
        writer.WriteInt(Height);
    }
    public void Load(IOReader reader)
    {
        Width  = reader.ReadInt();
        Height = reader.ReadInt();
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MapSize", indentation);
        indentation++;

        DebugFile.Log("Width:  " + Width,  indentation);
        DebugFile.Log("Height: " + Height, indentation);
    }
}
