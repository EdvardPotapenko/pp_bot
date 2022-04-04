using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace pp_bot.Achievements.Extensions;

internal static class LoggerExtensions
{
	public static void LogChatUserIsNull(this ILogger logger, Message message)
	{
		logger.LogWarning("Chat user with id {UserId} and chat {ChatId} is null",
			message.From!.Id, message.Chat.Id);
	}
}