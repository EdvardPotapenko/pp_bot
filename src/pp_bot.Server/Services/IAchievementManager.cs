using Telegram.Bot.Types;

namespace pp_bot.Server.Services;

public interface IAchievementManager
{
	Task HandleAchievementsAsync(Message m, CancellationToken ct);
}