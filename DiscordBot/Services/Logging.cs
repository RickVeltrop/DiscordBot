using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;

namespace DiscordBot.Services;

public class LoggingService
{
    private readonly DiscordSocketClient _client;

    private async Task LogAsync(LogMessage Msg)
    {
        var Severity = Msg.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };

        if (Msg.Exception is CommandException CmdException)
        {
            Log.Write(Severity, $"[Command/{Msg.Severity}] {CmdException.Command.Aliases[0]}"
                + $" failed to execute in {CmdException.Context.Channel}.");
            Log.Write(Severity, $"{CmdException}");
        }
        else
            Log.Write(Severity, Msg.Exception, $"[General/{Msg.Source}] {Msg.Message}");

        await Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        LogAsync(new LogMessage(
            LogSeverity.Info,
            "OnReady",
            $"Logged in as {_client.CurrentUser}"
        )).GetAwaiter().GetResult();

        await Task.CompletedTask;
    }

    public LoggingService(DiscordSocketClient Client)
    {
        _client = Client;
    }

    public Task InitializeAsync()
    {
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;

        return Task.CompletedTask;
    }
}