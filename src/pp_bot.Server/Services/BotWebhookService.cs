using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using pp_bot.Server.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services;

public sealed class BotWebhookService : IHostedService
{
	private readonly ITelegramBotClient _botClient;
	private readonly TelegramBotOptions _botOptions;

	public BotWebhookService(ITelegramBotClient botClient,
		IOptions<TelegramBotOptions> botOptions)
	{
		_botClient = botClient;
		_botOptions = botOptions.Value;

		if (_botOptions.WebhookOptions == null)
			throw new Exception("Webhook options are null");
	}
	
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		WebhookInfo webhookInfo = await _botClient.GetWebhookInfoAsync(cancellationToken);
		
		if (!string.IsNullOrEmpty(webhookInfo.Url))
		{
			// If webhook is already set - we need to delete it
			await _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
		}
		
		// Registering a new webhook
		var url = $"{_botOptions.WebhookOptions!.BaseUrl.TrimEnd('/')}/webhook/v1";
		await _botClient.SetWebhookAsync(url, cancellationToken: cancellationToken);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
	}
}