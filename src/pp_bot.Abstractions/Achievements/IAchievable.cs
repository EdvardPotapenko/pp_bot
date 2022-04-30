using Telegram.Bot.Types;

namespace pp_bot.Achievements;

public interface IAchievable
{
    Task AcquireAsync(Message m, CancellationToken ct);
}