using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot;

public class DiscordTarget : TargetWithLayout
{
    private readonly DiscordClient Client = Program._serviceProvider.GetRequiredService<DiscordClient>();
    public string? LogChannelID { get; set; }

    protected override void Write(LogEventInfo Event)
    {
        string Message = this.Layout.Render(Event);
        Client.GetChannelAsync(800726801470062592).Result.SendMessageAsync(Message);
    }
}
