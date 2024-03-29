﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace pp_bot.Server.Services
{
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
            await _client.ReceiveAsync(_updateHandler, stoppingToken);
        }
    }
}