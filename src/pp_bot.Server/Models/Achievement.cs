using System.Collections.Generic;

namespace pp_bot.Server.Models
{
    public class Achievement
    {
        public int Id {get;init;}    
        public List<BotUserChat> UsersAcquired{get;init;} = new List<BotUserChat>();
    }
}