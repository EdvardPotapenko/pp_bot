using System.Collections.Generic;

namespace pp_bot.Server.Models {
    public class Chat {
        public int Id{get;init;}
        public long ChatId {get;init;}
        public string ChatName{get;init;}
        public List<BotUser> ChatUsers{get;set;} = new List<BotUser>();
    }
}