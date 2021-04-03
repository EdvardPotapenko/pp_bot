using System;
using System.Collections.Generic;

namespace pp_bot.Server.Models
{
    public sealed class BotUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public long TelegramId { get; set; }

        public List<BotUserChat> UserChats { get; set; }
    }
}