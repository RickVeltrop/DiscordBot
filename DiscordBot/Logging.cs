using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot;

public class LoggingService
{
    private DiscordSocketClient _client;
    private CommandService _commands;

    private Task LogAsync(LogMessage Msg)
    {
        if (Msg.Exception is CommandException CmdException)
        {
            Console.WriteLine($"[Command/{Msg.Severity}] {CmdException.Command.Aliases.First()}"
                + $" failed to execute in {CmdException.Context.Channel}.");
            Console.WriteLine(CmdException);
        }
        else
            Console.WriteLine($"[General/{Msg.Severity}] {Msg}");

        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Console.WriteLine($"Logged in as {_client.CurrentUser}");

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