using System.Composition;
using pp_bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class ShowScoreCommand : IChatAction
{
    private readonly PP_Context _context;
    private readonly ITelegramBotClient _client;
    private readonly PPBotRepo _repo;

    private const string CommandName = "/score";

    public ShowScoreCommand(ITelegramBotClient client, PP_Context context)
    {
        _client = client;
        _context = context;
        _repo = new PPBotRepo(_context);
    }

    public bool Contains(Message message)
    {
        return message.Text.StartsWith(CommandName);
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct)
    {
        await ShowScoreAsync(message, ct);
    }

    private async Task ShowScoreAsync(Message message, CancellationToken ct)
    {
        var chat = await _repo.GetChatAsync(message,ct);

        if (chat.ChatUsers.Count == 0)
        {
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                "Челов пока нет в игре 🍆",
                cancellationToken: ct);
            return;
        }

        var topFifteen = chat.ChatUsers.OrderByDescending(u => u.PPLength).Take(15).ToList();

        var actualChat = await _client.GetChatAsync(message.Chat, ct);
        string scoreMessage = $"Топ 15 песюнов 🍆 в '{actualChat.Title}'\n";
        int i = 0;
        foreach (var botUser in topFifteen)
        {
            scoreMessage += $"🍆 {++i}. {botUser.User.Username} – {botUser.PPLength} см\n";
        }

        await _client.SendTextMessageAsync(
            message.Chat.Id,
            scoreMessage,
            Telegram.Bot.Types.Enums.ParseMode.Html,
            cancellationToken: ct);
    }
}