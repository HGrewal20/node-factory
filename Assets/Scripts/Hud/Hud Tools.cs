using System;

public static class HudTools
{
    public static string FormatTime(float seconds)
    {
        int totalSeconds = Math.Max(0, (int)seconds);

        int hh = totalSeconds / 3600;
        int mm = (totalSeconds / 60) % 60;
        int ss = totalSeconds % 60;

        return $"{hh:00}:{mm:00}:{ss:00}";
    }
}