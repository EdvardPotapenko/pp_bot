using pp_bot.Server.Achievements;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace pp_bot.Server.Сommands 
{
    public interface IChatAction 
    {
       Task ExecuteAsync(Message message, CancellationToken ct, IEnumerable<ITriggerable>? triggerables);

       bool Contains(Message message);
    }
}