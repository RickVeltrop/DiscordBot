using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DiscordBot;

internal class Program
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string? _token = Environment.GetEnvironmentVariable("TOKEN");

    public Program()
    {
        _serviceProvider = CreateProvider();
    }

    private static IServiceProvider CreateProvider()
    {
        var Config = new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Info,
            GatewayIntents = GatewayIntents.None,
        };

        return new ServiceCollection()
            .AddSingleton(Config)
            .AddSingleton<CommandService>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<LoggingService>()
            .BuildServiceProvider();
    }

    public static void Main(string[] args) =>
        new Program().MainAsync(args).GetAwaiter().GetResult();

    public async Task MainAsync(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        var _logging = _serviceProvider.GetRequiredService<LoggingService>();

        await _logging.InitializeAsync();

        await _client.LoginAsync(TokenType.Bot, _token, true);
        await _client.StartAsync();

        await Task.Delay(-1);
    }
}