using System.Composition;
using Microsoft.EntityFrameworkCore;
using pp_bot.Data;
using pp_bot.Data.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

// ReSharper disable InconsistentNaming

namespace pp_bot.Commands;

[Export(typeof(IChatAction))]
public sealed class GrowPPCommand : IChatAction
{
    private Random Random { get; } = new();
    private readonly PP_Context _context;
    private readonly ITelegramBotClient _client;

    private readonly PPBotRepo _repo;

    private const string CommandName = "/grow";

    private const int DelayMinutes = 60;

    public GrowPPCommand(ITelegramBotClient client, PP_Context context)
    {
        _client = client;
        _context = context;
        _repo = new PPBotRepo(context);
    }

    public bool Contains(Message message)
    {
        return message.Text.StartsWith(CommandName);
    }

    public async Task ExecuteAsync(Message message, CancellationToken ct)
    {

        var binding = await _context.BotUserChat
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.User.TelegramId == message.From.Id && b.UserChatsChatId == message.Chat.Id, ct);

        await ConfigurePP(binding, message);
    }

    private async Task ConfigurePP(BotUserChat binding, Message message)
    {
        var timePassed = Math.Abs((binding.LastManipulationTime - DateTime.Now).TotalMinutes);

        timePassed = Math.Round(timePassed, 1);

        if (timePassed < DelayMinutes)
        {
            var timeLeft = Math.Round(DelayMinutes - timePassed
                , 2);
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                $"{binding.User.Username}, не дергай ты так, а то оторвешь, потерпи ещё {timeLeft} мин.");
            return;
        }

        binding.LastManipulationTime = DateTime.Now;

        int ppManipulationResult = Random.Next(1, 10);
            
        bool sign = Convert.ToBoolean(Random.Next(0, 3));

        if (binding.PPLength - ppManipulationResult < 0 || sign)
        {
            binding.PPLength += Math.Abs(ppManipulationResult);
            binding.UserChatGrowHistory.Add(
                new GrowHistory()
                {
                    PPLengthChange = +ppManipulationResult
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
        binding.UserChatGrowHistory.Add(
            new GrowHistory()
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