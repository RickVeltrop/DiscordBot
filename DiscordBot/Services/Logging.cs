using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
namespace DiscordBot.Services;

public static class DiscordLogger
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static void Log(LogLevel Severity, InteractionContext ctx, string Message)
    {
        _logger.Log(Severity, $"Message logged to discord: {Message}");

        if (ctx.Guild == null)
            return;

        var LogChannel = ctx.Guild.Channels.FirstOrDefault(x => x.Value.Name == "gaylord-central").Value;

        if (LogChannel == null)
            LogChannel = ctx.Guild.CreateChannelAsync("gaylord-central", ChannelType.Text).Result;

        LogChannel.SendMessageAsync($"[Logger/{Severity.Name}]: {Message}");
    }
}
*/