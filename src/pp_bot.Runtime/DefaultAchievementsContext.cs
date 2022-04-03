using pp_bot.Achievements;

namespace pp_bot.Runtime;

internal sealed class DefaultAchievementsContext : IAchievementsContext
{
	private readonly IAchievementsLoader _achievementsLoader;
	private readonly IServiceProvider _serviceProvider;

	public DefaultAchievementsContext(IAchievementsLoader achievementsLoader, IServiceProvider serviceProvider)
	{
		_achievementsLoader = achievementsLoader;
		_serviceProvider = serviceProvider;
	}
	
	public IAchievable? GetAchievable(int id)
	{
		var factory = _achievementsLoader.AchievableFactory
			.FirstOrDefault(f => f.Metadata.Id == id);
		return factory?.CreateExport(_serviceProvider).Value;
	}

	public ITriggerable? GetTriggerable(int id)
	{
		var factory = _achievementsLoader.TriggerableFactory
			.FirstOrDefault(f => f.Metadata.Id == id);
		return factory?.CreateExport(_serviceProvider).Value;
	}

	public AchievementMetadata? GetAchievementMetadata(int id)
	{
		AchievementMetadata? metadata =
			_achievementsLoader.AchievableFactory.FirstOrDefault(f => f.Metadata.Id == id)?.Metadata ??
			_achievementsLoader.TriggerableFactory.FirstOrDefault(f => f.Metadata.Id == id)?.Metadata;
		
		return metadata;
	}
}