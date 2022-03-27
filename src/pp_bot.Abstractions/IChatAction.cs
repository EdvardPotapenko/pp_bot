using Telegram.Bot.Types;

namespace pp_bot.Abstractions;

public interface IChatAction 
{
	Task ExecuteAsync(Message message, CancellationToken ct, IEnumerable<ITriggerable>? triggerables);

	bool Contains(Message message);
}