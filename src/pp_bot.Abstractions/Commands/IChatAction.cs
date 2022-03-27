using Telegram.Bot.Types;

namespace pp_bot.Commands;

public interface IChatAction 
{
	Task ExecuteAsync(Message message, CancellationToken ct);

	bool Contains(Message message);
}