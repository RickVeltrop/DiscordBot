using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

internal class Program
{
    private readonly IServiceProvider _serviceProvider;
    private string? _token = Environment.GetEnvironmentVariable("TOKEN");

    public Program()
    {
        _serviceProvider = CreateProvider();
    }

    private static IServiceProvider CreateProvider()
    {
        var Config = new DiscordSocketConfig() { };

        var Collection = new ServiceCollection()
            .AddSingleton(Config)
            .AddSingleton<CommandService>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<LoggingService>();

        return Collection.BuildServiceProvider();
    }

    public static void Main(string[] args) =>
        new Program().MainAsync(args).GetAwaiter().GetResult();

    public async Task MainAsync(string[] args)
    {
        var _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        var _logging = _serviceProvider.GetRequiredService<LoggingService>();

        await _logging.InitializeAsync();

        await _client.LoginAsync(TokenType.Bot, _token, true);
        await _client.StartAsync();

        await Task.Delay(-1);
    }
}
