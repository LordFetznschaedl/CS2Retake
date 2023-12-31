using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace CS2Retake.Utils;

public static class MessageUtils
{
    private static readonly List<string> _thankYouMessages = new()
    {
        $"Thank you for using {ChatColors.Gold}CS2Retake{ChatColors.White} :)",
        "If you notice any bugs please report them here:",
        "https://github.com/LordFetznschaedl/CS2Retake/issues",
        "If you want to support the development of this Retake Plugin",
        "https://www.buymeacoffee.com/lordfetznschaedl"
    };

    public static string ModuleName { get; set; } = string.Empty;
    public static ILogger? Logger { get; set; }

    public static string PluginPrefix => $"[{ChatColors.Gold}{ModuleName}{ChatColors.White}]";

    public static void ThankYouMessage()
    {
        foreach (var message in _thankYouMessages) Server.PrintToChatAll($"{PluginPrefix} {message}");
    }

    public static void PrintToPlayerOrServer(string message, CCSPlayerController? player = null)
    {
        if (player != null)
        {
            message = $"{PluginPrefix} {message}";

            player.PrintToConsole(message);
            player.PrintToChat(message);
        }
        else
        {
            Log(LogLevel.Information, message);
        }
    }

    public static void PrintToChatAll(string message)
    {
        Server.PrintToChatAll($"{PluginPrefix} {message}");
    }

    public static void Log(LogLevel level, string? message, params object?[] args)
    {
        Logger?.Log(level, message, args);
    }
}