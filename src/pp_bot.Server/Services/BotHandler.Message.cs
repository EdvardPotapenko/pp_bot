﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Data;
using pp_bot.Data.Models;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services;

partial class BotHandler
{
    private async Task HandleMessageAsync(Message m, CancellationToken ct)
    {
        await _commandPatternManager.HandleCommandAsync(m, ct);
        await _achievementManager.HandleAchievementsAsync(m, ct);
    }
        
    private async Task HandleUserLeftAsync(Message m, CancellationToken ct)
    {
        try
        {
            if (m.LeftChatMember!.IsBot)
                return;

            long chatId = m.Chat.Id;
            long userId = m.LeftChatMember.Id;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PPContext>();

            context.BotUser__Chat.Remove(new Ref__BotUser__Chat {UserId = userId, ChatId = chatId});
            await context.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while removing the left user");
        }
    }
}