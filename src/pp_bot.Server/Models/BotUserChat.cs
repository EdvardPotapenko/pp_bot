using System;
using System.Collections.Generic;

namespace pp_bot.Server.Models
{
    public sealed class BotUserChat
    {
        public int ChatUsersId { get; set; }
        public BotUser User { get; set; }
        public long UserChatsChatId { get; set; }
        public Chat Chat { get; set; }
        public int PPLength { get; set; }
        public int LastPPLengthChange { get; set; }
        public DateTime LastManipulationTime { get; set; }
        public List<Achievement> AcquiredAchievements { get; set; } = new List<Achievement>();
    }
}