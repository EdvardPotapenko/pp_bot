using System.Collections.Generic;
using pp_bot.Server.Models;
using Telegram.Bot.Types.InputFiles;

namespace pp_bot.Server.DTO.Responses
{
    public class AchievementResponse
    {
        public int Id {get;init;}

        public string Name {get;init;}

        public string Description {get;init;}
        
        public InputOnlineFile Icon {get;init;}
        
        public List<BotUser> UsersAcquired{get;set;} = new List<BotUser>();
    }
}