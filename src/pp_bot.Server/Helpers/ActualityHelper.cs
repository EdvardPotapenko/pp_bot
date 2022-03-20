using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Chat = pp_bot.Server.Models.Chat;

namespace pp_bot.Server.Helpers;

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
            user = new BotUser
            {
                TelegramId = m.From.Id,
                DisplayName = displayName,
                Username = username
            };
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            context.BotUsers.Add(user);
            await context.SaveChangesAsync(ct);

            await BindUserAndChatAsync(context, new Chat { ChatId = m.Chat.Id }, user, ct);
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

    public static async Task BindUserAndChatAsync(PP_Context context, Chat chat, BotUser user, CancellationToken ct)
    {
        var botUserChat = new BotUserChat
        {
            ChatUsersId = user.Id,
            UserChatsChatId = chat.ChatId,
            LastManipulationTime = DateTime.Now
        };
        // ReSharper disable once MethodHasAsyncOverload
        context.BotUserChat.Add(botUserChat);
        await context.SaveChangesAsync(ct);
    }
}