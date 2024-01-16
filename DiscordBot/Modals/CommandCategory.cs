namespace DiscordBot.Modals;

public interface ICommandCategory
{
    public static string Name { get; set; }
    public static bool IsGlobal { get; set; }
}