using Discord.Commands;
using Serilog;
using System.Reflection;

namespace DiscordBot.Services;

public class Commands
{
    private Task RegisterCommandCategory()
    {
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        var Asm = Assembly.GetEntryAssembly();
        if (Asm == null)
        {
            Log.Error("[Commands/Initialization] Could not load commands due to Assembly.GetEntryAssembly() returning a null value.");
            return Task.CompletedTask;
        }

        Log.Information("[Commands/Initialization] Loading commands");

        var Classes = Asm.GetTypes().Where(Type => Type.IsClass && Type.IsSubclassOf(typeof(ModuleBase<SocketCommandContext>)));

        foreach (var Class in Classes)
        {
            RegisterCommandCategory();
        }

        Log.Information("[Commands/Initialization] Loaded commands");

        return Task.CompletedTask;
    }
}