using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace pp_bot.Server.Models
{
    public sealed class BotUserChat
    {
        public int ChatUsersId { get; set; }
        [ForeignKey("ChatUsersId")]
        public BotUser User { get; set; }
        public long UserChatsChatId { get; set; }
        [ForeignKey("UserChatsChatId")]
        public Chat Chat { get; set; }
        public int PPLength { get; set; }
        public DateTime LastManipulationTime { get; set; }
    }
}