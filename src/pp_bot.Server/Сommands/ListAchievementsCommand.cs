using System.Text;
using System.Threading;
using System.Threading.Tasks;
using pp_bot.Server.Helpers;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands
{
    /// <summary>
    /// Handles achievements command
    /// </summary>
    public class ListAchievementsCommand : IChatAction
    {
        private readonly PP_Context _context;
        private readonly ITelegramBotClient _client;
        private readonly DatabaseHelper _databaseHelper;
        private const string COMMAND_NAME = "/achievements";

        public ListAchievementsCommand(PP_Context context, ITelegramBotClient client)
        {
            _context = context;
            _client = client;
            _databaseHelper = new DatabaseHelper(context);
        }

        public async Task ExecuteAsync(Message m, CancellationToken ct)
        {
            var userChat = await _databaseHelper.GetUserChatAsync(m, ct);

            if(userChat.AcquiredAchievements.Count == 0)
            {
                
                await _client.SendTextMessageAsync
                (
                    m.Chat.Id,
                    $"У {userChat.User.Username} пока ещё нет достижений",
                    Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: ct
                );
                return;
            }

            StringBuilder achievementsMessage = new StringBuilder();
            achievementsMessage.Append($"Достижения <b>{userChat.User.Username}</b>" + "\n");

            // Concat all achievement info in one string message
            foreach (var achievement in userChat.AcquiredAchievements)
            {
                achievementsMessage.Append
                (
                    $"<b>{achievement.Name}</b>\n<i>{achievement.Description}</i>\nПользователей получило: {achievement.UsersAcquired.Count}\n"
                );
            }
        
            await _client.SendTextMessageAsync
            (
                m.Chat.Id,
                achievementsMessage.ToString(),
                Telegram.Bot.Types.Enums.ParseMode.Html,
                cancellationToken: ct
            );
        }
        public bool Contains(Message message)
        {
            return message.Text.StartsWith(COMMAND_NAME);
        }
    }
}