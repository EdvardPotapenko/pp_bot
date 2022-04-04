using System.Composition;
using Microsoft.Extensions.Logging;
using pp_bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class ShowScoreCommand : IChatAction
{
    private readonly ITelegramBotClient _client;
    private readonly PPBotRepo _repo;
    private readonly ILogger<ShowScoreCommand> _logger;

    private const string CommandName = "/score";

    public ShowScoreCommand(ITelegramBotClient client, PPBotRepo repo, ILogger<ShowScoreCommand> logger)
    {
        _client = client;
        _repo = repo;
        _logger = logger;
    }

    public bool Contains(Message message)
    {
        return message.Text?.StartsWith(CommandName) ?? false;
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct)
    {
        await ShowScoreAsync(message, ct);
    }

    private async Task ShowScoreAsync(Message message, CancellationToken ct)
    {
        var chat = await _repo.GetChatAsync(message,ct);

        if (chat == null)
        {
            _logger.LogWarning("Chat with id {ChatId} is null", message.Chat.Id);
            return;
        }

        if (chat.ChatUsers.Count == 0)
        {
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                "Челов пока нет в игре 🍆",
                cancellationToken: ct);
            return;
        }

        var topFifteen = chat.ChatUsers
            .OrderByDescending(u => u.PPLength)
            .Take(15)
            .ToList();

        var actualChat = await _client.GetChatAsync(message.Chat.Id, ct);
        string scoreMessage = $"Топ 15 песюнов 🍆 в '{actualChat.Title}'\n";
        int i = 0;
        foreach (var botUser in topFifteen)
        {
            scoreMessage += $"🍆 {++i}. {botUser.User.Username} – {botUser.PPLength} см\n";
        }

        await _client.SendTextMessageAsync(
            message.Chat.Id,
            scoreMessage,
            ParseMode.Html,
            cancellationToken: ct);
    }
}