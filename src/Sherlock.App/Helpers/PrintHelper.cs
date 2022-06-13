namespace Sherlock.App.Helpers;

/// <summary>
/// PrintHelper is a set of helper functions that facilitate the color-printing purpose on the console screen
/// </summary>
public static class PrintHelper
{
    public static void DumpWith(this string text, ConsoleColor color)
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = foregroundColor;
    }

    public static void RedDump(this string text)
        => text.DumpWith(ConsoleColor.Red);
    
    public static void GreenDump(this string text)
        => text.DumpWith(ConsoleColor.Green);

    public static void YellowDump(this string text)
        => text.DumpWith(ConsoleColor.Yellow);

    public static void DumpFor(this string text, bool debug)
    {
        if (!debug)
            return;

        text.DumpWith(ConsoleColor.DarkYellow);
    }
    
    public static void ErrorDumpFor(this string text, bool debug)
    {
        if (!debug)
            return;

        text.DumpWith(ConsoleColor.DarkRed);
    }
}