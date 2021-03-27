using System;
using System.Threading.Tasks;
using pp_bot.model;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.EntityFrameworkCore;

namespace pp_bot.bot.chat_actions.commands
{
    public class UserAPI
    {
        PP_Context _Context { get; set; }
        ITelegramBotClient _Client { get; set; }
        public UserAPI(ITelegramBotClient client, PP_Context context)
        {
            _Client = client;
            _Context = context;
        }
        public async Task<BotUser> HandleNewUserAsync(Message message, string waitMessage)
        {

            var user = await this.CreateNewUserAsync(message);

            await _Client.SendTextMessageAsync(
                message.Chat.Id,
                $"{user.Username} в игре 🎉\n" +
                waitMessage);
            return user;
        }

        public async Task DeleteBotUser(Message message){

            var userToDelete = await this.GetUserAsync(message);
            
            if(userToDelete == null)
                throw new ArgumentException("User to delete was not found");

            // delete user from all chats
            userToDelete.UserChats.ForEach(chat => {
                 chat.ChatUsers.Remove(userToDelete);
            });

            _Context.BotUsers.Remove(userToDelete);

            await _Context.SaveChangesAsync();
        }

        public async Task<BotUser?> GetUserAsync(Message message)
        {
            return await _Context.BotUsers.Include(u => u.UserChats)
                         .FirstOrDefaultAsync(user => user.TelegramId == message.From.Id);
        }

        public async Task<BotUser> CreateNewUserAsync(Message message)
        {
            string username = message.From.Username == null ? message.From.FirstName :
            message.From.Username;

            BotUser newUser = new BotUser()
            {
                TelegramId = message.From.Id,
                Username = username,
                PPLength = 0,
                LastManipulationTime = DateTime.Now
            };
            await _Context.BotUsers.AddAsync(newUser);
            await _Context.SaveChangesAsync();
            return newUser;
        }

        public async Task HandleChatBindingAsync(BotUser user, Message message)
        {
            model.Chat? chat = await this.GetChatAsync(message);

            if (chat == null)
            {
                chat = await this.CreateNewChatAsync(message);
                await this.BindUserAndChatAsync(chat, user);
                return;
            }

            if (chat.ChatUsers.Find(chatUser => chatUser.TelegramId == user.TelegramId) == null)
                await this.BindUserAndChatAsync(chat, user);
        }

        public async Task<model.Chat?> GetChatAsync(Message message)
        {
            return await _Context.Chats.Include(c => c.ChatUsers).FirstOrDefaultAsync(chat => chat.ChatId == message.Chat.Id);
        }

        public async Task BindUserAndChatAsync(model.Chat chat, BotUser user)
        {
            chat.ChatUsers.Add(user);
            user.UserChats.Add(chat);
            await _Context.SaveChangesAsync();
        }

        public async Task<model.Chat> CreateNewChatAsync(Message message)
        {
            model.Chat chat = new model.Chat()
            {
                ChatId = message.Chat.Id,
                ChatName = message.Chat.Title
            };
            await _Context.Chats.AddAsync(chat);
            await _Context.SaveChangesAsync();
            return chat;
        }
    }
}