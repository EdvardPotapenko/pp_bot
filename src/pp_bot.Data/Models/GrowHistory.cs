using System.ComponentModel.DataAnnotations;

namespace pp_bot.Data.Models;

public sealed record GrowHistory
{
    [Key]
    public long GrowHistoryId {get;init;}
    public int PPLengthChange { get; init; }
    public DateTime ExecutionTime{get;init;} = DateTime.UtcNow;
}