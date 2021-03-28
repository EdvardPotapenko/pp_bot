using System;
using System.Threading;
using System.Threading.Tasks;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands
{

    public class LeaveGameCommand : IChatAction
    {
        PP_Context _Context { get; set; }
        ITelegramBotClient _Client { get; set; }
        UserAPI _UserAPI { get; set; }

        const string _BotName = "@PPgrower_bot";
        const string _CommandName = "/leave";

        public LeaveGameCommand(ITelegramBotClient client, PP_Context context)
        {
            _Client = client;
            _Context = context;
            _UserAPI = new UserAPI(_Client, _Context);
        }

        public bool Contains(Message message)
        {
            return message.Text == _CommandName ||
                   message.Text == _CommandName + _BotName;
        }

        public async Task ExecuteAsync(Message message, CancellationToken ct)
        {
            if (message.Chat.Id > 0)
                return;

            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return;

            await this.LeaveGame(message);
        }

        private async Task LeaveGame(Message message)
        {
            try
            {
                await _UserAPI.DeleteBotUser(message);

                await _Client.SendTextMessageAsync(
                          message.Chat.Id,
                          $"Вжух! {message.From.FirstName} больше нет - как и не бывало!"
                          );
            }
            catch(ArgumentException){
                await _Client.SendTextMessageAsync(
                          message.Chat.Id,
                          $"Упс, не удалось удалить пользователя {message.From.FirstName}"
                          );
            }
        }
    }
}