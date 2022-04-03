using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Data;
using pp_bot.Runtime;
using pp_bot.Server.Helpers;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services;

public sealed class CommandPatternManager
{
    private readonly IServiceProvider _provider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ICommandsLoader _commandsLoader;

    public CommandPatternManager(IServiceProvider provider, ILoggerFactory loggerFactory, ICommandsLoader commandsLoader)
    {
        _provider = provider;
        _loggerFactory = loggerFactory;
        _commandsLoader = commandsLoader;
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

        foreach (var commandFactory in _commandsLoader.CommandFactory)
        {
            using var command = commandFactory.CreateExport(_provider);
            if (command.Value.Contains(m))
            {
                try
                {
                    await command.Value.ExecuteAsync(m, ct);
                }
                catch (Exception e)
                {
                    var logger = _loggerFactory.CreateLogger(commandFactory.GetType());
                    logger.LogError(e, "Exception occurred while running the command");
                }
                break;
            }
        }
    }
}