using Microsoft.EntityFrameworkCore;
using pp_bot.Data;
using pp_bot.Data.Models;
using Telegram.Bot.Types;
using Chat = pp_bot.Data.Models.Chat;

namespace pp_bot.Server.Helpers;

internal class ActualityHelper
{
    public static async Task EnsureUserIsActualAsync(Message m, PPContext context, CancellationToken ct)
    {
        string displayName = $"{m.From!.FirstName} {m.From.LastName}".Trim();
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

    public static async Task EnsureChatIsCreatedAsync(Message m, PPContext context, CancellationToken ct)
    {
        var chatExists = await context.Chats.AnyAsync(c => c.ChatId == m.Chat.Id, ct);
        if (!chatExists)
        {
            var chat = new Data.Models.Chat
            {
                ChatId = m.Chat.Id
            };
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            context.Chats.Add(chat);
            await context.SaveChangesAsync(ct);
        }
    }

    public static async Task BindUserAndChatAsync(PPContext context, Chat chat, BotUser user, CancellationToken ct)
    {
        var botUserChat = new Ref__BotUser__Chat
        {
            UserId = user.TelegramId,
            ChatId = chat.ChatId,
            UtcUpdatedAt = DateTime.Now
        };
        // ReSharper disable once MethodHasAsyncOverload
        context.BotUser__Chat.Add(botUserChat);
        await context.SaveChangesAsync(ct);
    }
}