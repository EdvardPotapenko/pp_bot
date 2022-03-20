using pp_bot.Server.Achievements;
using pp_bot.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pp_bot.Server.Helpers
{
    internal class DatabaseSeedingHelper
    {
        public async static Task EnsureAchievementsIntegrity(IEnumerable<IAchievable> achievements, IEnumerable<ITriggerable> triggerables, PP_Context context)
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
}
