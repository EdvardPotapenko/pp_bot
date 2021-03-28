using System;
using System.Threading;
using System.Threading.Tasks;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands
{

    public class GrowPPCommand : IChatAction
    {

        Random random { get; set; } = new Random();
        PP_Context _Context { get; set; }
        ITelegramBotClient _Client { get; set; }

        UserAPI _UserAPI {get;set;}

        const string _BotName = "@PPgrower_bot";
        const string _CommandName = "/grow";

        const int delayMinutes = 60;

        public GrowPPCommand(ITelegramBotClient client, PP_Context context)
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

            var user = await _UserAPI.GetUserAsync(message);

            if (user == null)
            {
                user = await _UserAPI.HandleNewUserAsync(
                    message,
                    $"Подожди ещё {delayMinutes} мин. чтобы начать ВЫРАЩИВАНИЕ!"
                    );
                await _UserAPI.HandleChatBindingAsync(user, message);
                return;
            }

            await _UserAPI.HandleChatBindingAsync(user, message);

            await this.ConfigurePP(user, message);
        }

        private async Task ConfigurePP(BotUser user, Message message)
        {
            var timePassed = Math.Abs((user.LastManipulationTime - DateTime.Now).TotalMinutes);

            timePassed = Math.Round(timePassed, 1);

            if (timePassed < delayMinutes)
            {
                var timeLeft = Math.Round(delayMinutes - timePassed
                , 2);
                await _Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"{user.Username}, не дергай ты так, а то оторвешь, потерпи ещё {timeLeft} мин.");
                return;
            }

            user.LastManipulationTime = DateTime.Now;

            int ppManipulationResult = random.Next(1, 10);

            bool sign = Convert.ToBoolean(random.Next(0, 3));

            if (user.PPLength - ppManipulationResult < 0 || sign)
            {
                user.PPLength += Math.Abs(ppManipulationResult);
                await _Context.SaveChangesAsync();
                await _Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Песюн 🍆 {user.Username} вырос 📈 на {ppManipulationResult}см\n" +
                    $"Теперь он {user.PPLength}см");
                return;
            }

            user.PPLength -= ppManipulationResult;
            await _Context.SaveChangesAsync();
            await _Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Песюн 🍆 {user.Username} отсох 🔻 на {ppManipulationResult}см\n" +
                    $"Тепер он {user.PPLength}см");
        }
    }
}