using pp_bot.Abstractions;
using pp_bot.Data.Models;

namespace pp_bot.Data.Helpers;

public static class DatabaseSeedingHelper
{
    public static async Task EnsureAchievementsIntegrity(IEnumerable<IAchievable> achievements, IEnumerable<ITriggerable> triggerables, PP_Context context)
    {
        foreach (var achievement in achievements)
        {
            if (context.Achievements.Any(a => a.Id == achievement.Id))
                continue;
            await context.AddAsync(new Achievement { Id = achievement.Id });
        }
        foreach (var triggerable in triggerables)
        {
            if (context.Achievements.Any(a => a.Id == triggerable.Id))
                continue;
            await context.AddAsync(new Achievement { Id = triggerable.Id });
        }
        await context.SaveChangesAsync();
    }
}