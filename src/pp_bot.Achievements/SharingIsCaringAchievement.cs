using System.Composition;
using Microsoft.Extensions.Logging;
using pp_bot.Achievements.Extensions;
using pp_bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Achievements;

[Export(typeof(ITriggerable))]
[ExportMetadata(AchievementMetadata.IdType, Id)]
[ExportMetadata(AchievementMetadata.NameType, Name)]
[ExportMetadata(AchievementMetadata.DescriptionType, Description)]
public sealed class SharingIsCaringAchievement : ITriggerable
{
    private readonly PPContext _context;
    private readonly PPBotRepo _repo;
    private readonly ILogger<SharingIsCaringAchievement> _logger;
    private readonly ITelegramBotClient _client;

    private const int Id = 5;
    private const string Name = "Поделись с другом :)";
    private const string Description = "Передать другому частицу своей души (члена) с помощью команды /transfer";

    public SharingIsCaringAchievement(ITelegramBotClient client, PPContext context, PPBotRepo repo,
        ILogger<SharingIsCaringAchievement> logger)
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

        await _context.AcquireAchievementAsync(Id, chatUser.Id, ct);

        await _client.SendTextMessageAsync(
            m.Chat.Id,
            $"<b>{chatUser.User.Username}</b> получил достижение <i>{Name}</i>, поздравляем 🎉!",
            ParseMode.Html,
            cancellationToken: ct);
    }
}