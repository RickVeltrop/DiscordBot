using DiscordBot.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using NLog;

namespace DiscordBot.Commands;

[SlashCommandGroup("dev", "Commands used for the development of the bot. Available on the development discord only.")]
public class GuildCommands : ApplicationCommandModule
{
    private readonly Logger _logger;

    public GuildCommands()
    {
        _logger = LogManager.GetCurrentClassLogger();
    }

    [SlashCommand("createlog", "Logging test command!")]
    public async Task TestCommand(InteractionContext ctx, [Option("message", "The log message.")] string Msg = "Hello")
    {
        await ctx.DeferAsync(false);

        _logger.Log(LogLevel.Info, Msg);

        await ctx.EditResponseAsync(
            new DiscordWebhookBuilder().WithContent("Done!")
        );
    }

    [SlashCommand("errortest", "Exception test!")]
    public async Task ExceptionCommand(InteractionContext ctx)
    {
        await ctx.DeferAsync(false);

        throw new NotImplementedException();

        await ctx.EditResponseAsync(
            new DiscordWebhookBuilder().WithContent("Done!")
        );
    }

    [SlashCommand("joinvc", "Make Gaylord join the specified voice chat.")]
    public async Task JoinVcCmd(InteractionContext ctx, [Option("channel", "The channel to join.")] DiscordChannel? Channel = null)
    {
        await ctx.DeferAsync(false);

        if (Channel == null && ctx.Member.VoiceState != null)
        {
            Channel = ctx.Member.VoiceState.Channel;
        }
        else if (Channel == null && ctx.Member.VoiceState == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You must select a voice channel or be connected to one in order to use this command."));
            _logger.Info("Not connected to a voice chat!");
            return;
        }

        if (Channel!.Type != ChannelType.Voice)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Channel option must be a voice channel."));
            return;
        }

        var VoiceClient = ctx.Client.GetVoiceNext();

        var VoiceConn = VoiceClient.GetConnection(ctx.Guild);
        if (VoiceConn != null)
            throw new InvalidOperationException("Already connected in this guild.");

        await Channel.ConnectAsync();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully joined #{Channel.Name}."));
    }

    [SlashCommand("leavevc", "Make Gaylord leave his curreny voice chat.")]
    public async Task Leave(InteractionContext ctx)
    {
        await ctx.DeferAsync(false);

        var VoiceClient = ctx.Client.GetVoiceNext();

        var VoiceConn = VoiceClient.GetConnection(ctx.Guild);
        if (VoiceConn == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I am not connected to a voice channel in this server."));
            return;
        }

        VoiceConn.Disconnect();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully left."));
    }
}
