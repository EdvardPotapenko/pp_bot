using System.Threading;
using System.Threading.Tasks;
using pp_bot.Server.Models;
using Telegram.Bot.Types;

namespace pp_bot.Server.Achievements
{
    public interface ITriggerable
    {
        int Id { get; }
        string Name { get; }
        string Description { get; }
        Task AcquireAsync(Message m, CancellationToken ct);
    }
}
