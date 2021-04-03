using System;
using System.Threading;
using System.Threading.Tasks;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands
{
    public sealed class LeaveGameCommand : IChatAction
    {
        private PP_Context Context { get; }
        private ITelegramBotClient Client { get; }
        private DatabaseHelper DatabaseHelper { get; }

        private const string CommandName = "/leave";

        public LeaveGameCommand(ITelegramBotClient client, PP_Context context)
        {
            Client = client;
            Context = context;
            DatabaseHelper = new DatabaseHelper(context);
        }

        public bool Contains(Message message)
        {
            return message.Text.StartsWith(CommandName);
        }

        public async Task ExecuteAsync(Message message, CancellationToken ct)
        {
            try
            {
                await DatabaseHelper.DeleteUserAsync(message);

                await Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Вжух! {message.From.FirstName} больше нет - как и не бывало!", cancellationToken: ct);
            }
            catch(ArgumentException)
            {
                await Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Упс, не удалось удалить пользователя {message.From.FirstName}", cancellationToken: ct);
            }
        }
    }
}