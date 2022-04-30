using System.ComponentModel.DataAnnotations.Schema;

namespace pp_bot.Data.Models;

[Table("achievements")]
public sealed class Achievement
{
    [Column("id")]
    public int Id { get; set; }
    
    public List<Ref__BotUserToChat__Achievement> UsersAcquired { get; set; }
}