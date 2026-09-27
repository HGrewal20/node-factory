using System;
using System.IO;
using UnityEngine;

public static class DebugFile
{
    public const int IndentSpaces = 2;

    private static string filePath;

    public static void Log(string text, int indent = 0)
    {
        EnsureFile();

        string indentation = new string(' ', indent * IndentSpaces);
        string output      = indentation + text + Environment.NewLine;

        File.AppendAllText(filePath, output);
    }

    private static void EnsureFile()
    {
        if (filePath != null)
            return;

        string folderPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Debug");
        Directory.CreateDirectory(folderPath);

        string fileName = $"Debug_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        filePath = Path.Combine(folderPath, fileName);
    }
}