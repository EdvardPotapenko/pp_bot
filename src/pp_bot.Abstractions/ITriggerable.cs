using Telegram.Bot.Types;

namespace pp_bot.Abstractions;

public interface ITriggerable
{
    int Id { get; }
    string Name { get; }
    string Description { get; }
    Task AcquireAsync(Message m, CancellationToken ct);
}