using System.ComponentModel.DataAnnotations;

namespace pp_bot.Server.Options;

public sealed class WebhookOptions
{
	[Required]
	public string BaseUrl { get; set; }
}