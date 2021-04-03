using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Models;
using Telegram.Bot.Types;
using Chat = pp_bot.Server.Models.Chat;
// ReSharper disable InconsistentNaming

namespace pp_bot.Server.Сommands
{
    public sealed class DatabaseHelper
    {
        private PP_Context Context { get; }
        
        public DatabaseHelper(PP_Context context)
        {
            Context = context;
        }

        public async Task DeleteUserAsync(Message message)
        {
            Context.BotUserChat.Remove(
                new BotUserChat {ChatUsersId = message.From.Id, UserChatsChatId = message.Chat.Id});
            await Context.SaveChangesAsync();
        }

        public async Task<BotUser> GetUserAsync(Message message)
        {
            return await Context.BotUsers
                .Include(u => u.UserChats)
                .ThenInclude(it => it.Chat)
                .FirstOrDefaultAsync(user => user.TelegramId == message.From.Id);
        }

        public async Task<Chat> GetChatAsync(Message message)
        {
            return await Context.Chats
                .Include(c => c.ChatUsers)
                .ThenInclude(it => it.User)
                .FirstOrDefaultAsync(chat => chat.ChatId == message.Chat.Id);
        }

        public async Task BindUserAndChatAsync(Chat chat, BotUser user)
        {
            var botUserChat = new BotUserChat
            {
                ChatUsersId = user.Id,
                UserChatsChatId = chat.ChatId,
                LastManipulationTime = DateTime.Now
            };
            // ReSharper disable once MethodHasAsyncOverload
            Context.BotUserChat.Add(botUserChat);
            await Context.SaveChangesAsync();
        }
    }
}