using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Targets;

namespace DiscordBot;

public class DiscordTarget : TargetWithLayout
{
    private readonly DiscordClient Client = Program._serviceProvider.GetRequiredService<DiscordClient>();
    public string? LogChannelID { get; set; }

    protected override void Write(LogEventInfo LogEvent)
    {
        ulong ChannelID;
        bool Converted = ulong.TryParse(LogChannelID, out ChannelID);

        if (!Converted)
        {
            throw new ArgumentException($"Configuration for LogChannelID was invalid; failed to convert {LogChannelID} to ulong.");
        }

        var Embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.DarkRed,
            Title = $"Uncaught {LogEvent.Level} occured;",
            Description = LogEvent.Message
        };

        Client.GetChannelAsync(ChannelID).Result.SendMessageAsync(Embed);
    }
}