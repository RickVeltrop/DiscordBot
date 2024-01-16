namespace DiscordBot.Modals;

public interface ICommand
{
    public static string Name { get; set; }
    public static bool IsGlobal { get; set; }
}