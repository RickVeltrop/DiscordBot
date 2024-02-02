using DiscordBot.Services;
using DSharpPlus;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace DiscordBot;

public static class Program
{
    private static readonly string? _token = Environment.GetEnvironmentVariable("TOKEN");
    public static readonly IServiceProvider _serviceProvider = CreateProvider();
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private static LoggingConfiguration LoggerConfig()
    {
        var _config = _serviceProvider.GetRequiredService<IConfiguration>();

        var LogConsole = new ConsoleTarget("logconsole");
        var LogFile = new FileTarget("logfile") { FileName = "${LogDir}/${LogDay}.log" };
        var LogDiscord = new DiscordTarget() { LogChannelID = _config.GetSection("LogChannel").Value };

        var config = new LoggingConfiguration();
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, LogConsole);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, LogFile);
        config.AddRule(LogLevel.Warn, LogLevel.Fatal, LogDiscord);

        return config;
    }

    private static IServiceProvider CreateProvider()
    {
        var BotConfig = new DiscordConfiguration()
        {
            Token = _token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
            LogUnknownEvents = false
        };

        var Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)!.FullName)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build().GetSection("AppSettings");

        return new ServiceCollection()
            .AddSingleton<IConfiguration>(Configuration)
            .AddSingleton(BotConfig)
            .AddSingleton<DiscordClient>()
            .AddSingleton<CommandHandler>()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddNLog();
            })
            .BuildServiceProvider();
    }

    public static void Main(string[] args) =>
        Program.MainAsync(args).GetAwaiter().GetResult();

    public static async Task MainAsync(string[] args)
    {
        var _client = _serviceProvider.GetRequiredService<DiscordClient>();
        var _commands = _serviceProvider.GetRequiredService<CommandHandler>();

        LogManager.Configuration = LoggerConfig();
        _client.UseVoiceNext();

        _client.Ready += OnClientConnect;

        await _commands.InitializeAsync();
        await _client.ConnectAsync();

        await Task.Delay(-1);
    }

    private static Task OnClientConnect(DiscordClient Client, DSharpPlus.EventArgs.ReadyEventArgs Args)
    {
        _logger.Info($"Logged in as {Client.CurrentUser.Username}#{Client.CurrentUser.Discriminator}");

        return Task.CompletedTask;
    }
}