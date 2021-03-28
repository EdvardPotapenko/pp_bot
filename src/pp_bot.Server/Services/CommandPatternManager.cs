using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using pp_bot.Server.Сommands;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services
{
    public sealed class CommandPatternManager
    {
        private readonly IServiceProvider _provider;

        public CommandPatternManager(IServiceProvider provider)
        {
            _provider = provider;
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
                    await command.ExecuteAsync(m, ct);
                    break;
                }
            }
        }
    }
}