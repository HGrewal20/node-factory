using UnityEngine;

public class MapSize
{
    // Static Variables
    public const int VALUE_MIN =  15;
    public const int VALUE_MAX = 200;

    public static readonly MapSize ZERO = new MapSize();

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

    // Is Valid
    public bool IsValid(int x, int y)
    {
        return (x >= 0) && (x < Width ) &&
               (y >= 0) && (y < Height);
    }

    public bool IsValid(RectInt rect)
    {
        return IsValid(rect.xMin    , rect.yMin    ) &&
               IsValid(rect.xMax - 1, rect.yMax - 1);
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
