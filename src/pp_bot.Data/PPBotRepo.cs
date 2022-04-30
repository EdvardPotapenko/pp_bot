using Microsoft.EntityFrameworkCore;
using pp_bot.Data.Models;
using Telegram.Bot.Types;
using Chat = pp_bot.Data.Models.Chat;
// ReSharper disable InconsistentNaming

namespace pp_bot.Data;

public sealed class PPBotRepo
{
    private readonly PPContext _context;

    public PPBotRepo(PPContext context)
    {
        _context = context;
    }

    public async Task DeleteUserAsync(Message message, CancellationToken ct)
    {
        _context.BotUser__Chat.Remove(
            new Ref__BotUser__Chat { UserId = message.From!.Id, ChatId = message.Chat.Id });
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Chat?> GetChatAsync(Message message, CancellationToken ct)
    {
        return await _context.Chats
            .AsNoTracking()
            .Include(c => c.ChatUsers)
            .ThenInclude(it => it.User)
            .FirstOrDefaultAsync(chat => chat.ChatId == message.Chat.Id, ct);
    }

    public async Task<Ref__BotUser__Chat?> GetChatUserAsync(Message message, CancellationToken ct)
    {
        return await _context.BotUser__Chat
            .AsNoTracking()
            .Include(uc => uc.User)
            .Include(uc => uc.AcquiredAchievements)
            .ThenInclude(a => a.Achievement.UsersAcquired)
            .Include(uc => uc.GrowHistory)
            .FirstOrDefaultAsync(uc => uc.Chat.ChatId == message.Chat.Id && uc.User.TelegramId == message.From!.Id, ct);
    }
}