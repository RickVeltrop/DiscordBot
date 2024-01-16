using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands;

public class TestCommands : ApplicationCommandModule
{
    private readonly Logger _logger;

    public TestCommands()
    {
        _logger = LogManager.GetCurrentClassLogger();
    }

    [SlashCommand("test", "A slash command made to test the DSharpPlusSlashCommands library!")]
    public async Task TestCommand(InteractionContext ctx)
    {
        _logger.Info($"Said Hiiiiiiii :3 to {ctx.User.Username}({ctx.User.Id})");

        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Hiiiiiiii :3")
        );
    }
}
