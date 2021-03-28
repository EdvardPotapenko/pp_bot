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
        PP_Context _Context { get; set; }
        ITelegramBotClient _Client { get; set; }
        UserAPI _UserAPI { get; set; }

        const string _BotName = "@PPgrower_bot";
        const string _CommandName = "/score";

        public ShowScoreCommand(ITelegramBotClient client, PP_Context context)
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

            await this.ShowScore(message);
        }

        private async Task ShowScore(Message message)
        {

            var chat = await _UserAPI.GetChatAsync(message);

            if (chat == null || chat.ChatUsers.Count == 0)
            {
                chat = await _UserAPI.CreateNewChatAsync(message);
                await _Client.SendTextMessageAsync(
                      message.Chat.Id,
                      "Челов пока нет в игре 🍆"
                      );
                return;
            }

            var topFifteen = chat.ChatUsers.OrderByDescending(u => u.PPLength).Take(15).ToList();

            string scoreMessage = $"Топ 15 песюнов 🍆 в '{chat.ChatName}'\n";

            topFifteen.ForEach((user) =>
            {
                scoreMessage += $"🍆 {user.Username} - {user.PPLength}см\n";
            });

            await _Client.SendTextMessageAsync(
                      message.Chat.Id,
                      scoreMessage,
                      parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                      );
        }
    }
}