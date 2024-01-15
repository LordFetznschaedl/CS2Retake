using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Configs;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace CS2Retake.Utils
{
    public static class MessageUtils
    {
        public static ILogger? Logger { get; set; }

        public static string PluginPrefix => $"[{ChatColors.Gold}{RuntimeConfig.ModuleName}{ChatColors.White}]";

        private static List<string> _thankYouMessages = new List<string>() 
        {
            $"Thank you for using {ChatColors.Gold}CS2Retake{ChatColors.White} :)", 
            $"If you notice any bugs please report them here:",
            $"https://github.com/LordFetznschaedl/CS2Retake/issues",
            $"If you want to support the development of this Retake Plugin",
            $"https://www.buymeacoffee.com/lordfetznschaedl",
        };

        public static void ThankYouMessage()
        {
            foreach(var message in _thankYouMessages)
            {
                Server.PrintToChatAll($"{MessageUtils.PluginPrefix} {message}");
            }
        }

        public static void PrintToPlayerOrServer(string message, CCSPlayerController? player = null)
        {
            if (player != null)
            {
                message = $"{MessageUtils.PluginPrefix} {message}";

                player.PrintToConsole(message);
                player.PrintToChat(message);
            }
            else
            {
                MessageUtils.Log(LogLevel.Information, message);
            }
        }

        public static void PrintToChatAll(string message)
        {
            Server.PrintToChatAll($"{MessageUtils.PluginPrefix} {message}");
        }

        public static void Log(LogLevel level, string? message, params object?[] args)
        {
            Logger?.Log(level, message, args);
        }

        public static void LogDebug(string? message, params object?[] args)
        {
            if(!FeatureConfig.EnableDebug)
            {
                return;
            }

            Logger?.LogInformation(message, args); 
        }
    }
}
