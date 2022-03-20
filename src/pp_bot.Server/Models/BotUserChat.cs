using System;
using System.Collections.Generic;

namespace pp_bot.Server.Models;

public sealed class BotUserChat
{
    public long ChatUsersId { get; set; }
    public BotUser User { get; set; }
    public long UserChatsChatId { get; set; }
    public Chat Chat { get; set; }
    public int PPLength { get; set; }
    public List<GrowHistory> UserChatGrowHistory {get;init;} = new List<GrowHistory>();
    public DateTime LastManipulationTime { get; set; }
    public List<Achievement> AcquiredAchievements { get; set; } = new List<Achievement>();
}