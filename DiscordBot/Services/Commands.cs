using DiscordBot.Commands;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using NLog;
using System.Reflection;
using System.Windows.Input;

namespace DiscordBot.Services;

public class CommandHandler
{
    private readonly Logger _logger;
    private readonly SlashCommandsExtension SlashCmdHandler;

    private Task RegisterCommandCategory(Type SlashCommandClass)
    {
        var GuildId = Environment.GetEnvironmentVariable("GUILD") ?? "764119388978413600";

        ulong? Guild = SlashCommandClass.Name == "GuildCommands" ? ulong.Parse(GuildId) : null;
        SlashCmdHandler.RegisterCommands(SlashCommandClass, Guild);

        return Task.CompletedTask;
    }

    private Task OnSlashCmdError(SlashCommandsExtension Sender, SlashCommandErrorEventArgs Args)
    {
        _logger.Error($"Uncaught exception {Args.Exception.GetType().Name} in /{Args.Context.QualifiedName}: ```\n{Args.Exception.ToString()}\n```");

        Args.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"An uncaught exception occured while running this command."));

        return Task.CompletedTask;
    }

    public CommandHandler(DiscordClient Client)
    {
        _logger = LogManager.GetCurrentClassLogger();
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
    }
}