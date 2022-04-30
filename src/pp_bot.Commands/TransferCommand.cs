using System.Composition;
using Microsoft.EntityFrameworkCore;
using pp_bot.Achievements.Exceptions;
using pp_bot.Data;
using pp_bot.Runtime;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class TransferCommand : IChatAction
{
    private readonly ITelegramBotClient _client;
    private readonly PPContext _context;
    private readonly IAchievementsContext _achievementsContext;

    public TransferCommand(ITelegramBotClient client, PPContext context, IAchievementsContext achievementsContext)
    {
        _client = client;
        _context = context;
        _achievementsContext = achievementsContext;
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct)
    {
        Task SendErrorAsync()
        {
            return _client.SendTextMessageAsync(message.Chat.Id,
                "Неверный формат команды, ожидается:\n`/transfer n @username`\nгде:\n" +
                "*n* \\- число сантиметров, которые необходимо передать \\(например, 20\\)\n" +
                "*@username* \\- юзернейм пользователя, которому передать сантиметры\\.\n\n" +
                "Также можно выполнить перевод, ответив на сообщение любого участника этой группы, " +
                "тогда юзернейм можно не указывать:\n`/transfer n`",
                ParseMode.MarkdownV2,
                replyToMessageId: message.MessageId,
                cancellationToken: ct);
        }

        string[] separatedText = message.Text?.Split(' ', 4) ?? Array.Empty<string>();
        if (separatedText.Length < 2)
        {
            await SendErrorAsync();
            return;
        }

        if (!int.TryParse(separatedText[1], out int valueToTransfer))
        {
            await SendErrorAsync();
            return;
        }

        if (valueToTransfer < 1)
        {
            await _client.SendTextMessageAsync(message.Chat.Id, "Минимально можно передать 1 см.",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        MessageEntity? userMention = message.Entities?.FirstOrDefault(e =>
            e.Type == MessageEntityType.TextMention);
        long targetUserId;
        if (userMention is { User: { } })
        {
            targetUserId = userMention.User.Id;
        }
        else if (separatedText.Length > 2 &&
                 (message.Entities?.Any(e => e.Type == MessageEntityType.Mention) ?? false))
        {
            var userByUsername = await _context.BotUsers
                .Where(u => u.Username == separatedText[2].Substring(1))
                .Select(u => new { u.TelegramId })
                .FirstOrDefaultAsync(ct);
            if (userByUsername == null)
            {
                await _client.SendTextMessageAsync(message.Chat.Id, "Получатель не зарегистрирован в боте!",
                    replyToMessageId: message.MessageId, cancellationToken: ct);
                return;
            }

            targetUserId = userByUsername.TelegramId;
        }
        else if (message.ReplyToMessage is { From: { } })
        {
            targetUserId = message.ReplyToMessage.From.Id;
        }
        else
        {
            await SendErrorAsync();
            return;
        }

        var binding = await _context.BotUser__Chat
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.User.TelegramId == message.From!.Id, ct);
        if (binding == null)
        {
            await _client.SendTextMessageAsync(message.Chat.Id, "Вы не зарегистрированы в боте!",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        if (binding.PPLength < valueToTransfer)
        {
            await _client.SendTextMessageAsync(message.Chat.Id, "Недостаточно длины на счету!",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        var targetUserBinding = await _context.BotUser__Chat
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.User.TelegramId == targetUserId, ct);
        if (targetUserBinding == null)
        {
            await _client.SendTextMessageAsync(message.Chat.Id, "Получатель не зарегистрирован в боте!",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        targetUserBinding.PPLength += valueToTransfer;
        binding.PPLength -= valueToTransfer;
        await _context.SaveChangesAsync(ct);

        var triggerable = _achievementsContext.GetTriggerable(5);
        if (triggerable == null)
            throw new AchievementNotFoundException(5);
                    
        await triggerable.AcquireAsync(message, ct);

        string sourceUserText = $"<a href=\"tg://user?id={message.From!.Id}\">{binding.User.Username}</a>";
        string targetUserText = $"<a href=\"tg://user?id={targetUserId}\">{targetUserBinding.User.Username}</a>";
        await _client.SendTextMessageAsync(message.Chat.Id,
            $"Не сказать, что {sourceUserText} отличается умом и " +
            $"сообразительностью, но он подарил {valueToTransfer} см {targetUserText}",
            ParseMode.Html,
            cancellationToken: ct);
    }

    public bool Contains(Message message) =>
        message.Text != null && message.Text.StartsWith("/transfer");
}