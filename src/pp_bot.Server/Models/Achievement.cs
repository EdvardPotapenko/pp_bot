using System.Collections.Generic; 

namespace pp_bot.Server.Models
{
    public class Achievement
    {
        public int Id {get;init;}

        public string Name {get;init;}

        public string Description {get;init;}
        
        public List<BotUser> UsersAcquired{get;set;} = new List<BotUser>();
    }
}