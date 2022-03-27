﻿using System.Composition;
using Microsoft.EntityFrameworkCore;
using pp_bot.Achievements.Exceptions;
using pp_bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Achievements;

[Export(typeof(IAchievable))]
[ExportMetadata(AchievementMetadata.IdType, Id)]
[ExportMetadata(AchievementMetadata.NameType, Name)]
[ExportMetadata(AchievementMetadata.DescriptionType, Description)]
public sealed class GloryToUkraineAchievement : IAchievable
{
    private readonly PP_Context _context;
    private readonly PPBotRepo _repo;
    private readonly ITelegramBotClient _client;

    private const int Id = 4;
    private const string Name = "Слава Україні! 🇺🇦";
    private const string Description = "Иметь флаг Украины в имени пользователя.";

    public GloryToUkraineAchievement(ITelegramBotClient client, PP_Context context, PPBotRepo repo)
    {
        _client = client;
        _context = context;
        _repo = repo;
    }
    public async Task AcquireAsync(Message m, CancellationToken ct)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.Id == Id, ct);

        if (achievement == null)
            throw new AchievementNotFoundException(Id);

        var userChat = await _repo.GetUserChatAsync(m, ct);

        if (userChat.AcquiredAchievements.Contains(achievement))
            return;

        if (userChat.User.DisplayName.Contains("🇺🇦"))
        {
            userChat.AcquiredAchievements.Add(achievement);
            achievement.UsersAcquired.Add(userChat);

            await _context.SaveChangesAsync(ct);

            await _client.SendTextMessageAsync(
                m.Chat.Id,
                $"<b>{userChat.User.Username}</b> получил достижение <i>{Name}</i>, поздравляем 🎉!",
                ParseMode.Html,
                cancellationToken: ct);
        }
    }
}