using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands {
    public interface IChatAction {
       Task ExecuteAsync(Message message, CancellationToken ct);

       bool Contains(Message message);
    }
}