using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands
{

    public class ShowScoreCommand : IChatAction
    {
        private PP_Context Context { get; set; }
        private ITelegramBotClient Client { get; set; }
        private UserAPI UserApi { get; set; }

        const string _BotName = "@PPgrower_bot";
        const string _CommandName = "/score";

        public ShowScoreCommand(ITelegramBotClient client, PP_Context context)
        {
            Client = client;
            Context = context;
            UserApi = new UserAPI(Client, Context);
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

            await ShowScore(message, ct);
        }

        private async Task ShowScore(Message message, CancellationToken ct)
        {

            var chat = await UserApi.GetChatAsync(message);

            if (chat == null || chat.ChatUsers.Count == 0)
            {
                chat = await UserApi.CreateNewChatAsync(message);
                await Client.SendTextMessageAsync(
                      message.Chat.Id,
                      "Челов пока нет в игре 🍆",
                      cancellationToken: ct);
                return;
            }

            var topFifteen = chat.ChatUsers.OrderByDescending(u => u.PPLength).Take(15).ToList();
            
            string scoreMessage = $"Топ 15 песюнов 🍆 в '{chat.ChatName}'\n";
            int i = 0;
            foreach (var botUser in topFifteen)
            {
                var actualUserInfo = await Client.GetChatMemberAsync(message.Chat, botUser.Id, ct);
                scoreMessage += $"🍆 {++i}. {actualUserInfo.User.Username} – {botUser.PPLength} см\n";
            }

            await Client.SendTextMessageAsync(
                      message.Chat.Id,
                      scoreMessage,
                      Telegram.Bot.Types.Enums.ParseMode.Html,
                      cancellationToken: ct);
        }
    }
}