using System;
using UnityEngine;

public interface IOReader : IDisposable
{
    // Constants
    public const byte INVALID = IOWriter.INVALID;

    // Interface
    bool    ReadBool  ();
    byte    ReadByte  ();
    int     ReadInt   ();
    long    ReadLong  ();
    float   ReadFloat ();
    string  ReadString();

    // Special - Unity
    public Vector2 ReadVector2()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        return new Vector2(x, y);
    }
    public Vector2Int ReadVector2Int()
    {
        int x = ReadInt();
        int y = ReadInt();
        return new Vector2Int(x, y);
    }
    public RectInt ReadRectInt()
    {
        int x      = ReadInt();
        int y      = ReadInt();
        int width  = ReadInt();
        int height = ReadInt();
        return new RectInt(x, y, width, height);
    }

    // Special - Game
    public Item ReadItem()
    {
        byte index = ReadByte();
        if (index == INVALID)   return null;
        else                    return ItemCollection.Get(index);
    }
    public MachineType ReadMachineType()
    {
        byte index = ReadByte();
        if (index == INVALID)   return null;
        else                    return MachineCollection.Get(index);
    }
    public Level ReadLevel()
    {
        byte index = ReadByte();
        if (index == INVALID)   return null;
        else                    return LevelCollection.Get(index);
    }
    public Recipe ReadRecipe()
    {
        byte index = ReadByte();
        if (index == INVALID)   return null;
        else                    return RecipeCollection.Get(index);
    }
}
