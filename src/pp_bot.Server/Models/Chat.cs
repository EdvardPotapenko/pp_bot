using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pp_bot.Server.Models
{
    public sealed record Chat
    {
        [Key]
        public long ChatId { get; init; }
        public List<BotUserChat> ChatUsers { get; init; }
    }
}