﻿using pp_bot.Server.Achievements;
using pp_bot.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pp_bot.Server.Helpers
{
    internal class DatabaseSeeder
    {
        public async static Task EnsureAchievementsIntegrity(IEnumerable<IAchievable> achievements, PP_Context context)
        {
            foreach (var achievement in achievements)
            {
                if (context.Achievements.Any(a => a.Id == achievement.Id))
                    continue;
                await context.AddAsync(new Achievement { Id = achievement.Id });
            }
            await context.SaveChangesAsync();
        }
    }
}
