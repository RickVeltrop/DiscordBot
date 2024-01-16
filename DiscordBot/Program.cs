using DSharpPlus;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;

namespace DiscordBot;

internal class Program
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Logger _logger;
    private static readonly string? _token = Environment.GetEnvironmentVariable("TOKEN");

    public Program()
    {
        _serviceProvider = CreateProvider();
        _logger = LogManager.GetCurrentClassLogger();
    }

    private static IServiceProvider CreateProvider()
    {
        var Config = new DiscordConfiguration()
        {
            Token = _token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        };

        return new ServiceCollection()
            .AddSingleton(Config)
            .AddSingleton<DiscordClient>()
            .AddSingleton<CommandHandler>()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();
            })
            .BuildServiceProvider();
    }

    public static void Main(string[] args) =>
        new Program().MainAsync(args).GetAwaiter().GetResult();

    public async Task MainAsync(string[] args)
    {
        var _client = _serviceProvider.GetRequiredService<DiscordClient>();
        var _commands = _serviceProvider.GetRequiredService<CommandHandler>();

        _client.Ready += OnClientConnect;

        await _commands.InitializeAsync();
        await _client.ConnectAsync();

        await Task.Delay(-1);
    }

    private async Task OnClientConnect(DiscordClient Client, DSharpPlus.EventArgs.ReadyEventArgs Args)
    {
        _logger.Info($"Logged in as {Client.CurrentUser.Username}#{Client.CurrentUser.Discriminator}");
    }
}
