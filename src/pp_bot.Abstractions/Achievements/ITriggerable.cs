using Telegram.Bot.Types;

namespace pp_bot.Achievements;

public interface ITriggerable
{
    Task AcquireAsync(Message m, CancellationToken ct);
}