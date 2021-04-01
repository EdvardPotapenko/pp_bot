using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Server.Сommands;
using Telegram.Bot.Types;

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

        public async ValueTask HandleCommandAsync(Message m, CancellationToken ct)
        {
            using var scope = _provider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

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
    }
}