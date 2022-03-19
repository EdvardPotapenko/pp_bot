using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Models;
using Telegram.Bot.Types;
using Chat = pp_bot.Server.Models.Chat;
// ReSharper disable InconsistentNaming

namespace pp_bot.Server.Helpers
{
    public sealed class PPBotRepo
    {
        private readonly PP_Context _context;

        public PPBotRepo(PP_Context context)
        {
            _context = context;
        }

        public async Task DeleteUserAsync(Message message, CancellationToken ct)
        {
            _context.BotUserChat.Remove(
                new BotUserChat { ChatUsersId = message.From.Id, UserChatsChatId = message.Chat.Id });
            await _context.SaveChangesAsync(ct);
        }

        public async Task<BotUser> GetUserAsync(Message message, CancellationToken ct)
        {
            return await _context.BotUsers
                .Include(u => u.UserChats)
                .ThenInclude(it => it.Chat)
                .FirstOrDefaultAsync(user => user.TelegramId == message.From.Id, ct);
        }

        public async Task<Chat> GetChatAsync(Message message, CancellationToken ct)
        {
            return await _context.Chats
                .Include(c => c.ChatUsers)
                .ThenInclude(it => it.User)
                .FirstOrDefaultAsync(chat => chat.ChatId == message.Chat.Id,ct);
        }

        public async Task<BotUserChat> GetUserChatAsync(Message message, CancellationToken ct)
        {
            return await _context.BotUserChat
                        .Include(uc => uc.User)
                        .Include(uc => uc.AcquiredAchievements)
                        .Include(uc => uc.UserChatGrowHistory)
                        .FirstOrDefaultAsync
                         (
                             uc => uc.Chat.ChatId == message.Chat.Id &&
                             uc.User.TelegramId == message.From.Id,
                             ct
                         );
        }

        public async Task BindUserAndChatAsync(Chat chat, BotUser user, CancellationToken ct)
        {
            var botUserChat = new BotUserChat
            {
                ChatUsersId = user.Id,
                UserChatsChatId = chat.ChatId,
                LastManipulationTime = DateTime.Now
            };
            // ReSharper disable once MethodHasAsyncOverload
            _context.BotUserChat.Add(botUserChat);
            await _context.SaveChangesAsync(ct);
        }
    }
}