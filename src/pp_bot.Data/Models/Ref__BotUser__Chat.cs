using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pp_bot.Data.Models;

[Table("bot_user__chat")]
// ReSharper disable once InconsistentNaming
public sealed class Ref__BotUser__Chat
{
    [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Column("user_id")]
    public long UserId { get; set; }
    public BotUser User { get; set; }
    
    [Column("chat_id")]
    public long ChatId { get; set; }
    public Chat Chat { get; set; }
    
    [Column("pp_length")]
    public int PPLength { get; set; }
    [Column("utc_updated_at", TypeName = "timestamp without time zone")]
    public DateTime UtcUpdatedAt { get; set; }
    
    public List<GrowHistory> GrowHistory { get; set; }
    public List<Ref__BotUserToChat__Achievement> AcquiredAchievements { get; set; }
}