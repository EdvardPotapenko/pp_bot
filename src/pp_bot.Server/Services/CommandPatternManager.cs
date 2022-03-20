using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Server.Achievements;
using pp_bot.Server.Helpers;
using pp_bot.Server.Models;
using pp_bot.Server.Сommands;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services;

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
            await ActualityHelper.EnsureChatIsCreatedAsync(m, context, ct);
            await ActualityHelper.EnsureUserIsActualAsync(m, context, ct);
        }
        catch (Exception e)
        {
            var logger = _loggerFactory.CreateLogger<CommandPatternManager>();
            logger.LogError(e, "Exception occurred while ensuring that user or chat is up-to-date");
            return;
        }

        IEnumerable<IChatAction> commands = scopedProvider.GetServices<IChatAction>();
        IEnumerable<ITriggerable> triggerables = scopedProvider.GetServices<ITriggerable>();
        foreach (var command in commands)
        {
            if (command.Contains(m))
            {
                try
                {
                    await command.ExecuteAsync(m, ct, triggerables);
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
}