using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Server.Achievements;
using pp_bot.Server.Helpers;
using pp_bot.Server.Models;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services;

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
            await ActualityHelper.EnsureChatIsCreatedAsync(m, context, ct);
            await ActualityHelper.EnsureUserIsActualAsync(m, context, ct);
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
}