using pp_bot.Abstractions;
using pp_bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Commands;

public sealed class LeaveGameCommand : IChatAction
{
    private readonly PP_Context _context;
    private readonly ITelegramBotClient _client;
    private readonly PPBotRepo _repo;

    private const string CommandName = "/leave";

    public LeaveGameCommand(ITelegramBotClient client, PP_Context context)
    {
        _client = client;
        _context = context;
        _repo = new PPBotRepo(context);
    }

    public bool Contains(Message message)
    {
        return message.Text.StartsWith(CommandName);
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct, IEnumerable<ITriggerable>? triggerables)
    {
        try
        {
            await _repo.DeleteUserAsync(message,ct);

            await _client.SendTextMessageAsync(
                message.Chat.Id,
                $"Вжух! {message.From.FirstName} больше нет - как и не бывало!", cancellationToken: ct);
        }
        catch(ArgumentException)
        {
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                $"Упс, не удалось удалить пользователя {message.From.FirstName}", cancellationToken: ct);
        }
    }
}