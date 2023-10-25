using Discord.Commands;
using Discord.Interactions;
using Serilog;

namespace DiscordBot.Commands;

public class TestCommands : ModuleBase<SocketCommandContext>
{
    private readonly IServiceProvider _serviceProvider;

    public TestCommands(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [SlashCommand("test", "a test command")]
    public async Task Test()
    {
        Log.Information("Test executed.");
        await ReplyAsync("Hiiiii :3");
    }
}