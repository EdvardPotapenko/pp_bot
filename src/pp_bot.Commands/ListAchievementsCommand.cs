using System.Composition;
using System.Text;
using pp_bot.Achievements.Exceptions;
using pp_bot.Data;
using pp_bot.Runtime;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class ListAchievementsCommand : IChatAction
{
    private readonly IAchievementsContext _achievementsContext;
    private readonly ITelegramBotClient _client;
    private readonly PPBotRepo _repo;
    
    private const string CommandName = "/achievements";

    public ListAchievementsCommand(IAchievementsContext achievementsContext, PPBotRepo repo, ITelegramBotClient client)
    {
        _achievementsContext = achievementsContext;
        _client = client;
        _repo = repo;
    }

    public async Task ExecuteAsync(Message m, CancellationToken ct)
    {
        var userChat = await _repo.GetUserChatAsync(m, ct);

        if(userChat.AcquiredAchievements.Count == 0)
        {
                
            await _client.SendTextMessageAsync
            (
                m.Chat.Id,
                $"У {userChat.User.Username} пока ещё нет достижений",
                Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: ct
            );
            return;
        }

        StringBuilder achievementsMessage = new StringBuilder();
        achievementsMessage.Append($"Достижения <b>{userChat.User.Username}</b>" + "\n");

        // Concat all achievement info in one string message
        foreach (var achievement in userChat.AcquiredAchievements)
        {
            var achievementMetadata = _achievementsContext.GetAchievementMetadata(achievement.Id);
            if (achievementMetadata == null)
                throw new AchievementNotFoundException(achievement.Id);

            achievementsMessage.Append(
                $"<b>{achievementMetadata.Name}</b>\n<i>{achievementMetadata.Description}</i>\n" +
                $"Пользователей получило: {achievement.UsersAcquired.Count}\n");
        }
        
        await _client.SendTextMessageAsync(
            m.Chat.Id,
            achievementsMessage.ToString(),
            Telegram.Bot.Types.Enums.ParseMode.Html,
            cancellationToken: ct);
    }
    public bool Contains(Message message)
    {
        return message.Text?.StartsWith(CommandName) ?? false;
    }
}