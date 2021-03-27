using System.Collections.Generic;

namespace pp_bot.model {
    public class Chat {
        public int Id{get;init;}
        public long ChatId {get;init;}
        public string ChatName{get;init;}
        public List<BotUser> ChatUsers{get;set;} = new List<BotUser>();
    }
}