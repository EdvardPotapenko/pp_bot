using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace pp_bot.Server.Achievements
{
    public interface IAchievable
    {
        Task IsAcquiredAsync(Message m, CancellationToken ct);
    }
}