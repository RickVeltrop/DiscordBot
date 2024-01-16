using Discord;
using Discord.WebSocket;
using DiscordBot.Modals;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using NLog;
using System.Reflection;
using System.Windows.Input;

namespace DiscordBot.Services;

public class CommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordClient _client;
    private readonly Logger _logger;
    private readonly SlashCommandsExtension SlashCmdHandler;

    private Task RegisterCommandCategory(Type SlashCommandClass)
    {
        SlashCmdHandler.RegisterCommands(SlashCommandClass, ulong.Parse(Environment.GetEnvironmentVariable("GUILD")!));

        return Task.CompletedTask;
    }

    public CommandHandler(IServiceProvider ServiceProvider, DiscordClient Client)
    {
        _serviceProvider = ServiceProvider;
        _client = Client;
        _logger = LogManager.GetCurrentClassLogger();

        SlashCmdHandler = _client.UseSlashCommands();
    }

    public async Task InitializeAsync()
    {
        var Asm = Assembly.GetEntryAssembly();
        if (Asm == null)
        {
            _logger.Error("[Commands/Initialization] Could not load commands due to Assembly.GetEntryAssembly() returning a null value.");
            return;
        }

        ulong ClientGuildID;
        bool Success = ulong.TryParse(Environment.GetEnvironmentVariable("GUILD"), out ClientGuildID);

        if (!Success)
            _logger.Error("[Commands/Initialization] Failed to get client's guild ID.");

        _logger.Info("[Commands/Initialization] Loading commands");

        var CmdCategories = Asm.GetTypes().Where(Type => Type.IsClass && typeof(ApplicationCommandModule).IsAssignableFrom(Type));
        if (!CmdCategories.Any())
            _logger.Warn("[Commands/Initialization] No command categories were found");

        foreach (var Category in CmdCategories)
        {
            await RegisterCommandCategory(Category);
        }

        _logger.Info("[Commands/Initialization] Loaded commands");

        await Task.CompletedTask;
    }
}