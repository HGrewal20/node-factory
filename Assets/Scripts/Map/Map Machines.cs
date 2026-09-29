using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapMachines
{
    // Member Variables
    private Machine[,] machines;    // Never null
    private long   [,] IDs;         // Never null

    // Constructor
    public MapMachines()
    {
        Clear();
    }

    // Clear
    public void Clear()
    {
        Set(0, 0);
    }

    // Set
    public void Set(MapSize size)           { Set(size.Width, size.Height); }
    public void Set(int width, int height)
    {
        machines = new Machine[width, height];
        IDs      = new long   [width, height];
    }

    // Get
    public int CountUsed()
    {
        int used   = 0;
        int width  = machines.GetLength(0);
        int height = machines.GetLength(1);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (machines[x, y] != null)
                    used++;

        return used;
    }

    public bool IsValid(Vector2Int position) { return IsValid(position.x, position.y); }
    public bool IsValid(int x, int y)
    {
        return x >= 0                       &&
               y >= 0                       &&
               x <  machines.GetLength(0)   &&
               y <  machines.GetLength(1);
    }

    public bool IsFree(RectInt rect) { return IsFree(rect.xMin, rect.yMin, rect.xMax, rect.yMax); }
    public bool IsFree(int x0, int y0, int x1, int y1)
    {
        for (int x = x0; x < x1; x++)
            for (int y = y0; y < y1; y++)
                if (machines[x, y] != null)
                    return false;

        return true;
    }

    public Machine Get(Vector2Int pos) { return Get(pos.x, pos.y); }
    public Machine Get(int x, int y)
    {
        return machines[x, y];
    }

    public Machine[]        GetArray(RectInt rect) { return GetSet(rect).ToArray(); }
    public HashSet<Machine> GetSet  (RectInt rect)
    {
        HashSet<Machine> result = new HashSet<Machine>();

        for (int x = rect.xMin; x < rect.xMax; x++)
            for (int y = rect.yMin; y < rect.yMax; y++)
                if (machines[x, y] != null)
                    result.Add(machines[x, y]);

        return result;
    }

    public Machine FindNext(Vector2Int position, Vector2Int direction, out Vector2Int foundPosition) { return FindNext(position, direction.x, direction.y, out foundPosition); }
    public Machine FindNext(Vector2Int position, int dx, int dy      , out Vector2Int foundPosition)
    {
        foundPosition = default;

        if (dx == 0 && dy == 0)
            return null;

        // Skip first point
        int x = position.x + dx;
        int y = position.y + dy;

        while (IsValid(x, y))
        {
            Machine machine = machines[x, y];
            if (machine != null)
            {
                foundPosition = new Vector2Int(x, y);
                return machine;
            }

            x += dx;
            y += dy;
        }

        return null;
    }

    // Add & Remove
    public  void Add   (Machine machine)                { Set(machine, machine.Rect); }
    public  void Remove(Machine machine)                { Set(   null, machine.Rect); }
    private void Set   (Machine machine, RectInt rect)
    {
        for (int x = rect.xMin; x < rect.xMax; x++)
            for (int y = rect.yMin; y < rect.yMax; y++)
                machines[x, y] = machine;
    }

    // Link
    public void Link(GameLinks links)
    {
        int width  = IDs.GetLength(0);
        int height = IDs.GetLength(1);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                long id = IDs[x, y];
                machines[x, y] = (id == 0) ? null : links.GetMachine(id);
            }
    }

    // IO
    public void Save(IOWriter writer)
    {
        int width  = machines.GetLength(0);
        int height = machines.GetLength(1);

        // Save Map Size
        writer.WriteInt(width );
        writer.WriteInt(height);

        // Save Machine IDs
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Machine machine = machines[x, y];
                long    id      = (machine == null) ? 0 : machine.ID;

                writer.WriteLong(id);
            }
    }

    public void Load(IOReader reader)
    {
        // Read Size
        int width  = reader.ReadInt();
        int height = reader.ReadInt();

        Set(width, height);

        // Read IDs
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                IDs[x, y] = reader.ReadLong();

        // Link() is called from outside immediately after loading.
    }

    // Debug
    public void Debug(int indentation)
    {
        DebugFile.Log("MapMachines", indentation);
        indentation++;

        int width  = machines.GetLength(0);
        int height = machines.GetLength(1);
        int used   = CountUsed();

        DebugFile.Log("Size: " + width + " x " + height, indentation);
        DebugFile.Log("Used: " + used                  , indentation);

        // Highest Y printed first so smallest Y appears at the bottom.
        for (int y = height - 1; y >= 0; y--)
        {
            string row = "";

            for (int x = 0; x < width; x++)
                row += (machines[x, y] == null) ? " " : "X";

            DebugFile.Log(row, indentation);
        }
    }
}