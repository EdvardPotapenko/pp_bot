using System.Composition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pp_bot.Achievements.Exceptions;
using pp_bot.Achievements.Extensions;
using pp_bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Achievements;

[Export(typeof(IAchievable))]
[ExportMetadata(AchievementMetadata.IdType, Id)]
[ExportMetadata(AchievementMetadata.NameType, Name)]
[ExportMetadata(AchievementMetadata.DescriptionType, Description)]
public sealed class TripleAchievement : IAchievable
{
    private readonly PPContext _context;
    private readonly PPBotRepo _repo;
    private readonly ILogger<TripleAchievement> _logger;
    private readonly ITelegramBotClient _client;

    private const int Id = 2;
    private const string Name = "ЙЕС, МИНУС ТРИ! ЮХУ!";
    private const string Description = "Получить -3 см. при выполнении комманды /grow";

    public TripleAchievement(ITelegramBotClient client, PPContext context, PPBotRepo repo,
        ILogger<TripleAchievement> logger)
    {
        _client = client;
        _context = context;
        _repo = repo;
        _logger = logger;
    }

    public async Task AcquireAsync(Message m, CancellationToken ct)
    {
        await _context.EnsureAchievementExistsAsync(Id, ct);

        var chatUser = await _repo.GetChatUserAsync(m, ct);
        if (chatUser == null)
        {
            _logger.LogChatUserIsNull(m);
            return;
        }

        if (chatUser.AcquiredAchievement(Id))
            return;

        if (chatUser.GrowHistory.Any(h => h.PPLengthChange == -3))
        {
            await _context.AcquireAchievementAsync(Id, chatUser.Id, ct);

            await _client.SendTextMessageAsync(
                m.Chat.Id,
                $"<b>{chatUser.User.Username}</b> получил достижение <i>{Name}</i>, поздравляем 🎉!",
                ParseMode.Html,
                cancellationToken: ct);
        }
    }
}