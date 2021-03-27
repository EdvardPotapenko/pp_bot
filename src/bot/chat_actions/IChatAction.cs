using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace pp_bot.bot.chat_actions {
    public interface IChatAction {
       Task Execute(Message message);

       bool Contains(Message message);
    }
}