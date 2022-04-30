using pp_bot.Commands;
using Telegram.Bot.Types;

namespace pp_bot.Runtime;

public interface ICommandsContext
{
	IChatAction? GetCommand(Message m);
}