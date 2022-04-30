using System.Composition;
using System.Text;
using Microsoft.Extensions.Logging;
using pp_bot.Achievements.Exceptions;
using pp_bot.Data;
using pp_bot.Runtime;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class ListAchievementsCommand : IChatAction
{
    private readonly IAchievementsContext _achievementsContext;
    private readonly ITelegramBotClient _client;
    private readonly ILogger<ListAchievementsCommand> _logger;
    private readonly PPBotRepo _repo;
    
    private const string CommandName = "/achievements";

    public ListAchievementsCommand(IAchievementsContext achievementsContext, PPBotRepo repo, ITelegramBotClient client,
        ILogger<ListAchievementsCommand> logger)
    {
        _achievementsContext = achievementsContext;
        _client = client;
        _logger = logger;
        _repo = repo;
    }

    public async Task ExecuteAsync(Message m, CancellationToken ct)
    {
        var userChat = await _repo.GetChatUserAsync(m, ct);

        if (userChat == null)
        {
            _logger.LogWarning("User chat for user {UserId} in chat {ChatId} is null",
                m.From!.Id, m.Chat.Id);
            return;
        }

        if (userChat.AcquiredAchievements.Count == 0)
        {
            await _client.SendTextMessageAsync(
                m.Chat.Id,
                $"У {userChat.User.Username} пока ещё нет достижений",
                ParseMode.Html,
                cancellationToken: ct);
            return;
        }

        var achievementsMessage = new StringBuilder();
        achievementsMessage.Append($"Достижения <b>{userChat.User.Username}</b>\n");

        // Concat all achievement info in one string message
        foreach (var achievement in userChat.AcquiredAchievements)
        {
            var achievementMetadata = _achievementsContext.GetAchievementMetadata(achievement.AchievementId);
            if (achievementMetadata == null)
                throw new AchievementNotFoundException(achievement.AchievementId);

            achievementsMessage.Append(
                $"<b>{achievementMetadata.Name}</b>\n<i>{achievementMetadata.Description}</i>\n" +
                $"Пользователей получило: {achievement.Achievement.UsersAcquired.Count}\n");
        }
        
        await _client.SendTextMessageAsync(
            m.Chat.Id,
            achievementsMessage.ToString(),
            ParseMode.Html,
            cancellationToken: ct);
    }
    
    public bool Contains(Message message)
    {
        return message.Text?.StartsWith(CommandName) ?? false;
    }
}