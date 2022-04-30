namespace pp_bot.Achievements.Exceptions;

public sealed class AchievementNotFoundException : Exception
{
	public AchievementNotFoundException(int achievementId)
		: base($"Achievement with id {achievementId} was not found")
	{
	}
}