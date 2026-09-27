using System;
using UnityEngine;

public interface IOWriter : IDisposable
{
    // Constants
    public const byte INVALID = 255;

    // Interface
    void WriteBool  (bool   value);
    void WriteByte  (byte   value);
    void WriteInt   (int    value);
    void WriteLong  (long   value);
    void WriteFloat (float  value);
    void WriteString(string value);

    // Special - Unity
    public void WriteVector2(Vector2 entry)
    {
        WriteFloat(entry.x);
        WriteFloat(entry.y);
    }
    public void WriteVector2Int(Vector2Int entry)
    {
        WriteInt(entry.x);
        WriteInt(entry.y);
    }
    public void WriteRectInt(RectInt entry)
    {
        WriteInt(entry.x     );
        WriteInt(entry.y     );
        WriteInt(entry.width );
        WriteInt(entry.height);
    }

    // Special - Game
    public void WriteItem(Item entry)
    {
        byte index = (entry == null) ? INVALID : entry.Index;
        WriteByte(index);
    }
    public void WriteMachineType(MachineType entry)
    {
        byte index = (entry == null) ? INVALID : entry.Index;
        WriteByte(index);
    }
    public void WriteLevel(Level entry)
    {
        byte index = (entry == null) ? INVALID : entry.Index;
        WriteByte(index);
    }
    public void WriteRecipe(Recipe entry)
    {
        byte index = (entry == null) ? INVALID : entry.Index;
        WriteByte(index);
    }
}
