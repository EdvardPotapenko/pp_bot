using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Helpers;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Achievements
{
    public class TripleAchievement : IAchievable
    {
        public string Name => "ЙЕС, МИНУС ТРИ! ЮХУ!";
        public string Description => "Получить -3 см. при выполнении комманды /grow";
        public int Id => 2;
        private readonly PP_Context _context;
        private readonly DatabaseHelper _databaseHelper;
        private readonly ITelegramBotClient _client;

        public TripleAchievement(ITelegramBotClient client, PP_Context context)
        {
            _client = client;
            _context = context;
            _databaseHelper = new DatabaseHelper(context);
        }

        public async Task AcquireAsync(Message m, CancellationToken ct)
        {
            var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Id == Id);

            if (achievement == null)
                throw new NotImplementedException($"Achievement with id {Id} was not found");

            var userChat = await _databaseHelper.GetUserChatAsync(m, ct);

            if (userChat.AcquiredAchievements.Contains(achievement))
                return;

            if (userChat.UserChatGrowHistory.Any(h => h.PPLengthChange == -3))
            {
                userChat.AcquiredAchievements.Add(achievement);
                achievement.UsersAcquired.Add(userChat);

                await _context.SaveChangesAsync(ct);

                await _client.SendTextMessageAsync
                (
                    m.Chat.Id,
                    $"<b>{userChat.User.Username}</b> получил достижение <i>{Name}</i>, поздравляем 🎉!",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: ct
                );
            }
        }
    }
}