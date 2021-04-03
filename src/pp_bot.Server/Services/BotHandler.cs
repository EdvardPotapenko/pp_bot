using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Server.Services
{
    public sealed partial class BotHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _client;
        private readonly CommandPatternManager _commandPatternManager;
        private readonly IAchievementManager _achievementManager;

        public BotHandler(ITelegramBotClient client, CommandPatternManager commandPatternManager,
            IAchievementManager achievementManager)
        {
            _client = client;
            _commandPatternManager = commandPatternManager;
            _achievementManager = achievementManager;
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await (update switch
            {
                { Message: { Chat: { Type: ChatType.Group or ChatType.Supergroup }, Text: { } } } =>
                    HandleMessageAsync(update.Message, cancellationToken),
                _ => Task.CompletedTask
            });
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }

        public UpdateType[] AllowedUpdates => new[] {UpdateType.Message};
    }
}