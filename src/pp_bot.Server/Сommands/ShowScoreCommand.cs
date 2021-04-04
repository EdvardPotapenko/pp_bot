using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands
{
    public sealed class ShowScoreCommand : IChatAction
    {
        private PP_Context Context { get; }
        private ITelegramBotClient Client { get; }
        private DatabaseHelper DatabaseHelper { get; }

        private const string CommandName = "/score";

        public ShowScoreCommand(ITelegramBotClient client, PP_Context context)
        {
            Client = client;
            Context = context;
            DatabaseHelper = new DatabaseHelper(Context);
        }

        public bool Contains(Message message)
        {
            return message.Text.StartsWith(CommandName);
        }

        public async Task ExecuteAsync(Message message, CancellationToken ct)
        {
            await ShowScore(message, ct);
        }

        private async Task ShowScore(Message message, CancellationToken ct)
        {
            var chat = await DatabaseHelper.GetChatAsync(message);

            if (chat.ChatUsers.Count == 0)
            {
                await Client.SendTextMessageAsync(
                      message.Chat.Id,
                      "Челов пока нет в игре 🍆",
                      cancellationToken: ct);
                return;
            }

            var topFifteen = chat.ChatUsers.OrderByDescending(u => u.PPLength).Take(15).ToList();

            var actualChat = await Client.GetChatAsync(message.Chat, ct);
            string scoreMessage = $"Топ 15 песюнов 🍆 в '{actualChat.Title}'\n";
            int i = 0;
            foreach (var botUser in topFifteen)
            {
                scoreMessage += $"🍆 {++i}. {botUser.User.Username} – {botUser.PPLength} см\n";
            }

            await Client.SendTextMessageAsync(
                      message.Chat.Id,
                      scoreMessage,
                      Telegram.Bot.Types.Enums.ParseMode.Html,
                      cancellationToken: ct);
        }
    }
}