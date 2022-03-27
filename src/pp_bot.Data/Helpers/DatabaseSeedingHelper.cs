using System.Linq;
using pp_bot.Achievements;
using pp_bot.Data.Models;
using pp_bot.Runtime;

namespace pp_bot.Data.Helpers;

public static class DatabaseSeedingHelper
{
    public static async Task EnsureAchievementsIntegrity(IAchievementsLoader achievementsLoader, PP_Context context)
    {
        foreach (var achievementFactory in achievementsLoader.AchievableFactory)
        {
            var achievementMetadata = achievementFactory.Metadata;
            if (context.Achievements.Any(a => a.Id == achievementMetadata.Id))
                continue;
            await context.AddAsync(new Achievement { Id = achievementMetadata.Id });
        }
        
        foreach (var triggerableFactory in achievementsLoader.TriggerableFactory)
        {
            var achievementMetadata = triggerableFactory.Metadata;
            if (context.Achievements.Any(a => a.Id == achievementMetadata.Id))
                continue;
            await context.AddAsync(new Achievement { Id = achievementMetadata.Id });
        }
        
        await context.SaveChangesAsync();
    }
}