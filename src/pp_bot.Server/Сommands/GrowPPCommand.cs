using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Chat = pp_bot.Server.Models.Chat;

// ReSharper disable InconsistentNaming

namespace pp_bot.Server.Сommands
{
    public class GrowPPCommand : IChatAction
    {
        private Random Random { get; } = new();
        private PP_Context Context { get; }
        private ITelegramBotClient Client { get; }

        private DatabaseHelper DatabaseHelper { get; }

        private const string CommandName = "/grow";

        private const int DelayMinutes = 60;

        public GrowPPCommand(ITelegramBotClient client, PP_Context context)
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
            var binding = await Context.BotUserChat
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.User.TelegramId == message.From.Id && b.UserChatsChatId == message.Chat.Id, ct);

            if (binding == null)
            {
                var user = await Context.BotUsers.AsNoTracking().FirstAsync(u => u.TelegramId == message.From.Id, ct);
                await DatabaseHelper.BindUserAndChatAsync(new Chat {ChatId = message.Chat.Id}, user);
                await Client.SendTextMessageAsync(
                    message.Chat,
                    $"Подожди ещё {DelayMinutes} мин. чтобы начать ВЫРАЩИВАНИЕ!",
                    cancellationToken: ct);
                return;
            }

            await ConfigurePP(binding, message);
        }

        private async Task ConfigurePP(BotUserChat binding, Message message)
        {
            var timePassed = Math.Abs((binding.LastManipulationTime - DateTime.Now).TotalMinutes);

            timePassed = Math.Round(timePassed, 1);

            if (timePassed < DelayMinutes)
            {
                var timeLeft = Math.Round(DelayMinutes - timePassed
                , 2);
                await Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"{binding.User.Username}, не дергай ты так, а то оторвешь, потерпи ещё {timeLeft} мин.");
                return;
            }

            binding.LastManipulationTime = DateTime.Now;

            int ppManipulationResult = Random.Next(1, 10);

            bool sign = Convert.ToBoolean(Random.Next(0, 3));

            if (binding.PPLength - ppManipulationResult < 0 || sign)
            {
                binding.PPLength += Math.Abs(ppManipulationResult);
                await Context.SaveChangesAsync();
                await Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Песюн 🍆 {binding.User.Username} вырос 📈 на {ppManipulationResult} см\n" +
                    $"Теперь он {binding.PPLength} см");
                return;
            }

            binding.PPLength -= ppManipulationResult;
            await Context.SaveChangesAsync();
            await Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Песюн 🍆 {binding.User.Username} отсох 🔻 на {ppManipulationResult} см\n" +
                    $"Тепер он {binding.PPLength} см");
        }
    }
}