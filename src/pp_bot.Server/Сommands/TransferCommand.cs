using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pp_bot.Server.Achievements;
using pp_bot.Server.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace pp_bot.Server.Сommands;

public sealed class TransferCommand : IChatAction
{
    private readonly ITelegramBotClient _client;
    private readonly PP_Context _context;

    public TransferCommand(ITelegramBotClient client, PP_Context context)
    {
        _client = client;
        _context = context;
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct, IEnumerable<ITriggerable>? triggerables)
    {
        Task SendErrorAsync()
        {
            return _client.SendTextMessageAsync(message.Chat,
                "Неверный формат команды, ожидается:\n`/transfer n @username`\nгде:\n" +
                "*n* \\- число сантиметров, которые необходимо передать \\(например, 20\\)\n" +
                "*@username* \\- юзернейм пользователя, которому передать сантиметры\\.\n\n" +
                "Также можно выполнить перевод, ответив на сообщение любого участника этой группы, " +
                "тогда юзернейм можно не указывать:\n`/transfer n`",
                ParseMode.MarkdownV2,
                replyToMessageId: message.MessageId,
                cancellationToken: ct);
        }

        string[] separatedText = message.Text.Split(' ', 4);
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
            await _client.SendTextMessageAsync(message.Chat, "Минимально можно передать 1 см.",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        MessageEntity userMention = message.Entities?.FirstOrDefault(e =>
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
                await _client.SendTextMessageAsync(message.Chat, "Получатель не зарегистрирован в боте!",
                    replyToMessageId: message.MessageId, cancellationToken: ct);
                return;
            }

            targetUserId = (int)userByUsername.TelegramId;
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

        var binding = await _context.BotUserChat
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.User.TelegramId == message.From.Id, ct);
        if (binding == null)
        {
            await _client.SendTextMessageAsync(message.Chat, "Вы не зарегистрированы в боте!",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        if (binding.PPLength < valueToTransfer)
        {
            await _client.SendTextMessageAsync(message.Chat, "Недостаточно длины на счету!",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        var targetUserBinding = await _context.BotUserChat
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.User.TelegramId == targetUserId, ct);
        if (targetUserBinding == null)
        {
            await _client.SendTextMessageAsync(message.Chat, "Получатель не зарегистрирован в боте!",
                replyToMessageId: message.MessageId, cancellationToken: ct);
            return;
        }

        targetUserBinding.PPLength += valueToTransfer;
        binding.PPLength -= valueToTransfer;
        await _context.SaveChangesAsync(ct);

        // trigger ShareingIsCaring
        await triggerables.First(t => t.Id == 5).AcquireAsync(message, ct);

        string sourceUserText = $"<a href=\"tg://user?id={message.From.Id}\">{binding.User.Username}</a>";
        string targetUserText = $"<a href=\"tg://user?id={targetUserId}\">{targetUserBinding.User.Username}</a>";
        await _client.SendTextMessageAsync(message.Chat,
            $"Не сказать, что {sourceUserText} отличается умом и " +
            $"сообразительностью, но он подарил {valueToTransfer} см {targetUserText}",
            ParseMode.Html,
            cancellationToken: ct);
    }

    public bool Contains(Message message) =>
        message.Text != null && message.Text.StartsWith("/transfer");
}