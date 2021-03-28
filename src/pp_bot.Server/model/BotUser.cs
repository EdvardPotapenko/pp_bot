using System;
using System.Collections.Generic;

namespace pp_bot.Server.model {
    public class BotUser {
        public int Id{get;init;}
        public string Username{get;set;}
        public long TelegramId {get;init;}
        public long PPLength{get;set;}
        public DateTime LastManipulationTime{get;set;}

        public List<Chat> UserChats{get;set;} = new List<Chat>();
    }
}