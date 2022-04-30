using System.Composition;
using Microsoft.EntityFrameworkCore;
using pp_bot.Data;
using pp_bot.Data.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class GrowPPCommand : IChatAction
{
    private static readonly Random Random = new();
    
    private readonly PPContext _context;
    private readonly ITelegramBotClient _client;

    private const string CommandName = "/grow";
    private const int DelayMinutes = 60;

    public GrowPPCommand(ITelegramBotClient client, PPContext context)
    {
        _client = client;
        _context = context;
    }

    public bool Contains(Message message)
    {
        return message.Text?.StartsWith(CommandName) ?? false;
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct)
    {
        var binding = await _context.BotUser__Chat
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.User.TelegramId == message.From!.Id && b.ChatId == message.Chat.Id, ct);
        
        if (binding == null)
            return;

        await ConfigurePP(binding, message);
    }

    private async Task ConfigurePP(Ref__BotUser__Chat binding, Message message)
    {
        var timePassed = Math.Round(Math.Abs((binding.UtcUpdatedAt - DateTime.Now).TotalMinutes), 1);

        if (timePassed < DelayMinutes)
        {
            var timeLeft = Math.Round(DelayMinutes - timePassed, 2);
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                $"{binding.User.Username}, не дергай ты так, а то оторвешь, потерпи ещё {timeLeft} мин.");
            return;
        }

        binding.UtcUpdatedAt = DateTime.UtcNow;

        int ppManipulationResult = Random.Next(1, 10);
        bool sign = Convert.ToBoolean(Random.Next(0, 3));

        if (sign || binding.PPLength - ppManipulationResult < 0)
        {
            binding.PPLength += ppManipulationResult;
            binding.GrowHistory.Add(
                new GrowHistory
                {
                    PPLengthChange = ppManipulationResult
                }
            );

            await _context.SaveChangesAsync();
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                $"Песюн 🍆 {binding.User.Username} вырос 📈 на {ppManipulationResult} см\n" +
                $"Теперь он {binding.PPLength} см");         
            return;
        }

        binding.PPLength -= ppManipulationResult;
        binding.GrowHistory.Add(
            new GrowHistory
            {
                PPLengthChange = -ppManipulationResult
            }
        );
        await _context.SaveChangesAsync();
        await _client.SendTextMessageAsync(
            message.Chat.Id,
            $"Песюн 🍆 {binding.User.Username} отсох 🔻 на {ppManipulationResult} см\n" +
            $"Тепер он {binding.PPLength} см");
    }
}