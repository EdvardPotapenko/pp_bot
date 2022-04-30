using System.ComponentModel.DataAnnotations;

namespace pp_bot.Server.Options;

public sealed class TelegramBotOptions
{
	[Required]
	public string Token { get; set; }
	
	[Required]
	public ListenKind ListenKind { get; set; }
	
	public WebhookOptions? WebhookOptions { get; set; }
}

public enum ListenKind
{
	Polling,
	Webhook
}