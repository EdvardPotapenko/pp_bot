using pp_bot.Achievements;

namespace pp_bot.Runtime;

public interface IAchievementsContext
{
	IAchievable? GetAchievable(int id);
	ITriggerable? GetTriggerable(int id);
	
	AchievementMetadata? GetAchievementMetadata(int id);
}