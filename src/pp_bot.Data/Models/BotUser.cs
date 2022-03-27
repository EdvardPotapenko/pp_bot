namespace pp_bot.Data.Models;

public sealed class BotUser
{
    public int Id { get; set; }
    public string Username { get; set; }

    public string DisplayName { get; set; }
    public long TelegramId { get; init; }

    public List<BotUserChat> UserChats { get; set; }
}