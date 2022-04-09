#if WEBHOOK
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace pp_bot.Server.Controllers;

[ApiController]
[Route("webhook/v1")]
public sealed class WebhookController : ControllerBase
{
	private readonly IUpdateHandler _updateHandler;
	private readonly ITelegramBotClient _botClient;

	public WebhookController(IUpdateHandler updateHandler, ITelegramBotClient botClient)
	{
		_updateHandler = updateHandler;
		_botClient = botClient;
	}

	[HttpPost]
	public async Task<ActionResult> HandleUpdate(Update update)
	{
		try
		{
			await _updateHandler.HandleUpdateAsync(_botClient, update, HttpContext.RequestAborted);
		}
		catch (ApiRequestException ex)
		{
			await _updateHandler.HandleErrorAsync(_botClient, ex, HttpContext.RequestAborted);
			return StatusCode(500);
		}

		return Ok();
	}
}
#endif