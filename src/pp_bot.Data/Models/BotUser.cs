using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pp_bot.Data.Models;

[Table("bot_users")]
public sealed class BotUser
{
    [Key, Column("telegram_id")]
    public long TelegramId { get; set; }
    [Column("username")]
    public string? Username { get; set; }
    [Required, Column("display_name")]
    public string DisplayName { get; set; }

    public List<Ref__BotUser__Chat> UserChats { get; set; }
}