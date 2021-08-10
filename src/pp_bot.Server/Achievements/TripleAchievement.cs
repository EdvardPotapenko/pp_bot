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
        public Achievement Achievement{get;init;}
        private const int ACHIEVEMENT_ID = 2;
        private readonly PP_Context _context;
        private readonly DatabaseHelper _databaseHelper;
        private readonly ITelegramBotClient _client;

        public TripleAchievement(ITelegramBotClient client, PP_Context context)
        {
            _client = client;
            _context = context;
            _databaseHelper = new DatabaseHelper(context);

            Achievement = _context.Achievements.FirstOrDefault(a => a.Id == ACHIEVEMENT_ID);

            if (Achievement == null)
                throw new NotImplementedException($"Achievement with id {ACHIEVEMENT_ID} was not found");
        }

        public async Task AcquireAsync(Message m, CancellationToken ct)
        {
            var userChat = await _databaseHelper.GetUserChatAsync(m, ct);

            if (userChat.AcquiredAchievements.Contains(Achievement))
                return;

            if (userChat.UserChatGrowHistory.Any(h => h.PPLengthChange == -3))
            {
                userChat.AcquiredAchievements.Add(Achievement);
                Achievement.UsersAcquired.Add(userChat);

                await _context.SaveChangesAsync(ct);

                await _client.SendTextMessageAsync
                (
                    m.Chat.Id,
                    $"<b>{userChat.User.Username}</b> получил достижение <i>{Achievement.Name}</i>, поздравляем 🎉!",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: ct
                );
            }
        }
    }
}