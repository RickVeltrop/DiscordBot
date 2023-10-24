using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot;

public class LoggingService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    private Task LogAsync(LogMessage Msg)
    {
        if (Msg.Exception is CommandException CmdException)
        {
            Console.WriteLine($"[Command/{Msg.Severity}] {CmdException.Command.Aliases[0]}"
                + $" failed to execute in {CmdException.Context.Channel}.");
            Console.WriteLine(CmdException);
        }
        else
            Console.WriteLine($"[General/{Msg.Severity}] {Msg}");

        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        LogAsync(new LogMessage(
            LogSeverity.Info,
            "OnReady",
            $"Logged in as {_client.CurrentUser}"
        )).GetAwaiter().GetResult();

        return Task.CompletedTask;
    }

    public LoggingService(DiscordSocketClient Client, CommandService Cmds)
    {
        _client = Client;
        _commands = Cmds;
    }

    public Task InitializeAsync()
    {
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _commands.Log += LogAsync;

        return Task.CompletedTask;
    }
}