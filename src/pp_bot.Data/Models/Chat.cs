using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pp_bot.Data.Models;

[Table("chats")]
public sealed class Chat
{
    [Key, Column("chat_id")]
    public long ChatId { get; set; }
    
    public List<Ref__BotUser__Chat> ChatUsers { get; set; }
}