using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Helpers;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Achievements;

public class FirstHundredAchievement : IAchievable
{
    public string Name { get; } = "Саня, верни сотку.";
    public string Description { get; } = "Отрастить длинну агрегата в 100 см.";
    public int Id { get; } = 1;
    private readonly PP_Context _context;
    private readonly PPBotRepo _repo;
    private readonly ITelegramBotClient _client;

    public FirstHundredAchievement(ITelegramBotClient client, PP_Context context)
    {
        _client = client;
        _context = context;
        _repo = new PPBotRepo(context);
    }
    public async Task AcquireAsync(Message m, CancellationToken ct)
    {
        var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Id == Id);

        if (achievement == null)
            throw new NotImplementedException($"Achievement with id {Id} was not found");

        var userChat = await _repo.GetUserChatAsync(m, ct);

        if (userChat.AcquiredAchievements.Contains(achievement))
            return;

        if (userChat.PPLength >= 100)
        {
            userChat.AcquiredAchievements.Add(achievement);
            achievement.UsersAcquired.Add(userChat);

            await _context.SaveChangesAsync(ct);

            await _client.SendTextMessageAsync
            (
                m.Chat.Id,
                $"<b>{userChat.User.Username}</b> получил достижение <i>{Name}</i>, поздравляем 🎉!",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: ct
            );
        }
    }
}