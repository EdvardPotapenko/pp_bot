using Microsoft.EntityFrameworkCore;
using pp_bot.Achievements.Exceptions;
using pp_bot.Data;
using pp_bot.Data.Models;

namespace pp_bot.Achievements.Extensions;

internal static class AchievementExtensions
{
	public static async Task EnsureAchievementExistsAsync(this PPContext context, int achievementId,
		CancellationToken ct)
	{
		if (!await context.Achievements.AnyAsync(a => a.Id == achievementId, ct))
			throw new AchievementNotFoundException(achievementId);
	}

	public static bool AcquiredAchievement(this Ref__BotUser__Chat chatUser, int achievementId)
	{
		return chatUser.AcquiredAchievements.Any(@ref => @ref.AchievementId == achievementId);
	}

	public static async Task AcquireAchievementAsync(this PPContext context, int achievementId, long chatUserId,
		CancellationToken ct)
	{
		context.BotUserToChat__Achievement.Add(new Ref__BotUserToChat__Achievement
		{
			AchievementId = achievementId,
			ChatUserId = chatUserId
		});
		await context.SaveChangesAsync(ct);
	}
}