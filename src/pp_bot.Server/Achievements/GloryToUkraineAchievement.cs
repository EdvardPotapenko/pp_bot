using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Helpers;
using pp_bot.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Server.Achievements
{

    public class GloryToUkraineAchievement : IAchievable
    {
        public string Name => "Слава Україні! 🇺🇦";
        public string Description => "Иметь флаг Украины в имени пользователя.";
        public int Id => 4;
        private readonly PP_Context _context;
        private readonly PPBotRepo _repo;
        private readonly ITelegramBotClient _client;

        public GloryToUkraineAchievement(ITelegramBotClient client, PP_Context context)
        {
            _client = client;
            _context = context;
            _repo = new PPBotRepo(context);
        }
        public async Task AcquireAsync(Message m, CancellationToken ct)
        {
            var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Id == Id);

            if (achievement == null)
                throw new NotImplementedException($"Achievement with id {Id} was not found");

            var userChat = await _repo.GetUserChatAsync(m, ct);

            if (userChat.AcquiredAchievements.Contains(achievement))
                return;

            if (userChat.User.DisplayName.Contains("🇺🇦"))
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
