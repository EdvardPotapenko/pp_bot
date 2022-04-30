using System.Composition;
using pp_bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class LeaveGameCommand : IChatAction
{
    private readonly ITelegramBotClient _client;
    private readonly PPBotRepo _repo;

    private const string CommandName = "/leave";

    public LeaveGameCommand(ITelegramBotClient client, PPBotRepo repo)
    {
        _client = client;
        _repo = repo;
    }

    public bool Contains(Message message)
    {
        return message.Text?.StartsWith(CommandName) ?? false;
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct)
    {
        try
        {
            await _repo.DeleteUserAsync(message,ct);

            await _client.SendTextMessageAsync(
                message.Chat.Id,
                $"Вжух! {message.From!.FirstName} больше нет - как и не бывало!", cancellationToken: ct);
        }
        catch (ArgumentException) // TODO maybe not ArgumentException?
        {
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                $"Упс, не удалось удалить пользователя {message.From!.FirstName}", cancellationToken: ct);
        }
    }
}