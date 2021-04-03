using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Server.Models;
using pp_bot.Server.Сommands;
using Telegram.Bot.Types;
using Chat = pp_bot.Server.Models.Chat;

namespace pp_bot.Server.Services
{
    public sealed class CommandPatternManager
    {
        private readonly IServiceProvider _provider;
        private readonly ILoggerFactory _loggerFactory;

        public CommandPatternManager(IServiceProvider provider, ILoggerFactory loggerFactory)
        {
            _provider = provider;
            _loggerFactory = loggerFactory;
        }

        public async Task HandleCommandAsync(Message m, CancellationToken ct)
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

            IEnumerable<IChatAction> commands = scopedProvider.GetServices<IChatAction>();
            foreach (var command in commands)
            {
                if (command.Contains(m))
                {
                    try
                    {
                        await command.ExecuteAsync(m, ct);
                    }
                    catch (Exception e)
                    {
                        var logger = _loggerFactory.CreateLogger(command.GetType());
                        logger.LogError(e, "Exception occurred while running the command");
                    }
                    break;
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
                    ChatId = m.Chat.Id,
                    ChatName = m.Chat.Title
                };
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                context.Chats.Add(chat);
                await context.SaveChangesAsync(ct);
            }
        }
    }
}