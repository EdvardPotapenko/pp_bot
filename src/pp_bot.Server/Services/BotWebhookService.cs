#if WEBHOOK
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace pp_bot.Server.Services;

public sealed class BotWebhookService : IHostedService
{
	private readonly ITelegramBotClient _botClient;
	private readonly IUpdateHandler _updateHandler;

	public BotWebhookService(ITelegramBotClient botClient, IUpdateHandler updateHandler)
	{
		_botClient = botClient;
		_updateHandler = updateHandler;
	}
	
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var webhookInfo = await _botClient.GetWebhookInfoAsync(cancellationToken);
		
		if (!string.IsNullOrEmpty(webhookInfo.Url))
		{
			// If webhook is already set - we need to delete it
			await _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
		}
		
		// Registering a new webhook
		await _botClient.SetWebhookAsync("", cancellationToken: cancellationToken);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		
	}
}
#endif