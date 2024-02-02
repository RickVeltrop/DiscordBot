using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Reflection;

namespace DiscordBot.Services;

public class CommandHandler
{
    private readonly Logger _logger;
    private readonly IConfiguration _config;
    private readonly SlashCommandsExtension SlashCmdHandler;

    private Task RegisterCommandCategory(Type SlashCommandClass)
    {
        var HomeGuildId = _config.GetSection("HomeGuild").Value;
        ulong GuildId;
        bool Converted = ulong.TryParse(HomeGuildId, out GuildId);

        if (!Converted)
        {
            throw new ArgumentException($"Configuration for HomeGuildId was invalid; failed to convert {HomeGuildId} to ulong.");
        }

        ulong? Guild = SlashCommandClass.Name == "GuildCommands" ? GuildId : null;
        SlashCmdHandler.RegisterCommands(SlashCommandClass, Guild);

        return Task.CompletedTask;
    }

    private Task OnSlashCmdError(SlashCommandsExtension Sender, SlashCommandErrorEventArgs Args)
    {
        _logger.Error($"Uncaught exception {Args.Exception.GetType().Name} in /{Args.Context.QualifiedName}: ```\n{Args.Exception.ToString()}\n```");

        Args.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"An uncaught exception occured while running this command."));

        return Task.CompletedTask;
    }

    public CommandHandler(DiscordClient Client, IConfiguration Config)
    {
        _logger = LogManager.GetCurrentClassLogger();
        _config = Config;

        SlashCmdHandler = Client.UseSlashCommands();
        SlashCmdHandler.SlashCommandErrored += OnSlashCmdError;
    }

    public async Task InitializeAsync()
    {
        var Asm = Assembly.GetEntryAssembly();
        if (Asm == null)
        {
            _logger.Error("[Commands/Initialization] Could not load commands due to Assembly.GetEntryAssembly() returning a null value.");
            return;
        }

        _logger.Info("[Commands/Initialization] Loading commands");

        var CmdCategories = Asm.GetTypes().Where(Type => Type.IsClass && typeof(ApplicationCommandModule).IsAssignableFrom(Type));
        if (!CmdCategories.Any())
            _logger.Warn("[Commands/Initialization] No command categories were found");

        foreach (var Category in CmdCategories)
        {
            await RegisterCommandCategory(Category);
        }

        _logger.Info("[Commands/Initialization] Loaded commands");
    }
}