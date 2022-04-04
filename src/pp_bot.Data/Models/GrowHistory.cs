using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pp_bot.Data.Models;

[Table("grow_history")]
public sealed class GrowHistory
{
	[Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	[Column("pp_length_change")]
	public int PPLengthChange { get; set; }
	[Column("utc_created_at", TypeName = "timestamp without timezone")]
	public DateTime UtcCreatedAt { get; set; } = DateTime.UtcNow;
	
	public Ref__BotUser__Chat User { get; set; }
}