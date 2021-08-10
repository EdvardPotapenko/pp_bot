using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace pp_bot.Server.Achievements
{
    public interface IAchievable
    {
        Task AcquireAsync(Message m, CancellationToken ct);
    }
}