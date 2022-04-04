using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pp_bot.Data.Models;

[Table("bot_user_to_chat__achievement")]
// ReSharper disable once InconsistentNaming
public sealed class Ref__BotUserToChat__Achievement
{
	[Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	
	[Column("achievement__id")]
	public int AchievementId { get; set; }
	public Achievement Achievement { get; set; }
	
	[Column("bot_user_to_chat__id")]
	public long ChatUserId { get; set; }
	public Ref__BotUser__Chat ChatUser { get; set; }
}