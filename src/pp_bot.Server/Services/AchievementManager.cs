using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Data;
using pp_bot.Runtime;
using pp_bot.Server.Helpers;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services;

public sealed class AchievementManager : IAchievementManager
{
    private readonly IServiceProvider _provider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IAchievementsLoader _achievementsLoader;

    public AchievementManager(IServiceProvider provider, ILoggerFactory loggerFactory,
        IAchievementsLoader achievementsLoader)
    {
        _provider = provider;
        _loggerFactory = loggerFactory;
        _achievementsLoader = achievementsLoader;
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

        foreach (var achievementFactory in _achievementsLoader.AchievableFactory)
        {
            try
            {
                using var achievement = achievementFactory.CreateExport(_provider);
                await achievement.Value.AcquireAsync(m, ct);
            }
            catch (Exception e)
            {
                var logger = _loggerFactory.CreateLogger(achievementFactory.GetType());
                logger.LogError(e, "Exception occurred while checking achievement");
            }
        }
    }
}