using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Server.Achievements;
using pp_bot.Server.Models;
using Telegram.Bot.Types;
using Chat = pp_bot.Server.Models.Chat;

namespace pp_bot.Server.Services
{
    public interface IAchievementManager
    {
        Task HandleAchievementsAsync(Message m, CancellationToken ct);
    }

    public sealed class AchievementManager : IAchievementManager
    {
        private readonly IServiceProvider _provider;
        private readonly ILoggerFactory _loggerFactory;

        public AchievementManager(IServiceProvider provider, ILoggerFactory loggerFactory)
        {
            _provider = provider;
            _loggerFactory = loggerFactory;
        }

        public async Task HandleAchievementsAsync(Message m, CancellationToken ct)
        {
            using var scope = _provider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            try
            {
                var context = scopedProvider.GetRequiredService<PP_Context>();
                await EnsureUserIsActualAsync(m, context, ct);
                await EnsureChatIsCreatedAsync(m, context, ct);
            }
            catch (Exception e)
            {
                var logger = _loggerFactory.CreateLogger<CommandPatternManager>();
                logger.LogError(e, "Exception occurred while ensuring that user or chat is up-to-date");
                return;
            }

            IEnumerable<IAchievable> achievements = scopedProvider.GetServices<IAchievable>();
            foreach (var achievement in achievements)
            {
                try
                {
                    await achievement.AcquireAsync(m, ct);
                }
                catch (Exception e)
                {
                    var logger = _loggerFactory.CreateLogger(achievement.GetType());
                    logger.LogError(e, "Exception occurred while checking achievement");
                }
            }
        }

        private static async Task EnsureUserIsActualAsync(Message m, PP_Context context, CancellationToken ct)
        {
            string username = m.From.Username ?? m.From.FirstName;
            var user = await context.BotUsers
                .FirstOrDefaultAsync(u => u.TelegramId == m.From.Id, ct);

            if (user == null)
            {
                var newUser = new BotUser
                {
                    TelegramId = m.From.Id,
                    Username = username
                };
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                context.BotUsers.Add(newUser);
                await context.SaveChangesAsync(ct);
            }
            else if (user.Username != username)
            {
                user.Username = username;
                await context.SaveChangesAsync(ct);
            }
        }

        private static async Task EnsureChatIsCreatedAsync(Message m, PP_Context context, CancellationToken ct)
        {
            var chatExists = await context.Chats.AnyAsync(c => c.ChatId == m.Chat.Id, ct);
            if (!chatExists)
            {
                var chat = new Chat
                {
                    ChatId = m.Chat.Id
                };
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                context.Chats.Add(chat);
                await context.SaveChangesAsync(ct);
            }
        }
    }
}