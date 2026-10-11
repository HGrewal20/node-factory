using System.IO;
using UnityEngine;

public static class GameSavePaths
{
    public static string GetLevelPath(int levelIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"level_{levelIndex}.save");
    }

    public static bool Exists(int levelIndex)
    {
        return File.Exists( GetLevelPath(levelIndex) );
    }

    public static void Delete(int levelIndex)
    {
        string path = GetLevelPath(levelIndex);

        if (File.Exists(path))
            File.Delete(path);
    }
}