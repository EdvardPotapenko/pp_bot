#if POLLING
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Server.Services;

public sealed class BotHandlerService : BackgroundService
{
    private readonly ITelegramBotClient _client;
    private readonly IUpdateHandler _updateHandler;

    public BotHandlerService(ITelegramBotClient client, IUpdateHandler updateHandler)
    {
        _client = client;
        _updateHandler = updateHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _client.ReceiveAsync(_updateHandler,
            new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            },
            stoppingToken);
    }
}
#endif