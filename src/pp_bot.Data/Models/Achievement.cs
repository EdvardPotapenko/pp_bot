namespace pp_bot.Data.Models;

public class Achievement
{
    public int Id { get; init; }
    public List<BotUserChat> UsersAcquired { get; init; }
}