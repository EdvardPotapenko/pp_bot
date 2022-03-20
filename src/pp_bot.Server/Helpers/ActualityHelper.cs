using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace pp_bot.Server.Helpers
{
    internal class ActualityHelper
    {
        public static async Task EnsureUserIsActualAsync(Message m, PP_Context context, CancellationToken ct)
        {
            string displayName = $"{m.From.FirstName} {m.From.LastName}".Trim();
            string username = m.From.Username ?? m.From.FirstName;
            var user = await context.BotUsers
                .FirstOrDefaultAsync(u => u.TelegramId == m.From.Id, ct);

            if (user is null)
            {
                var newUser = new BotUser
                {
                    TelegramId = m.From.Id,
                    DisplayName = displayName,
                    Username = username
                };
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                context.BotUsers.Add(newUser);
                await context.SaveChangesAsync(ct);
                return;
            }

            if (user.Username != username)
            {
                user.Username = username;
                await context.SaveChangesAsync(ct);
            }
            if (user.DisplayName != displayName)
            {
                user.DisplayName = displayName;
                await context.SaveChangesAsync(ct);
            }

        }

        public static async Task EnsureChatIsCreatedAsync(Message m, PP_Context context, CancellationToken ct)
        {
            var chatExists = await context.Chats.AnyAsync(c => c.ChatId == m.Chat.Id, ct);
            if (!chatExists)
            {
                var chat = new Models.Chat
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
