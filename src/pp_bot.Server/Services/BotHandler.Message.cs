using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace pp_bot.Server.Services
{
    partial class BotHandler
    {
        private async Task HandleMessageAsync(Message m, CancellationToken ct)
        {
            await Task.Yield();
        }
    }
}