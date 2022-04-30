using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pp_bot.Achievements;
using pp_bot.Runtime.Composition;

namespace pp_bot.Runtime;

public interface IAchievementsLoader
{
	IEnumerable<ExportFactoryDependencyInjection<IAchievable, AchievementMetadata>> AchievableFactory { get; }
	IEnumerable<ExportFactoryDependencyInjection<ITriggerable, AchievementMetadata>> TriggerableFactory { get; }
	
	void LoadAchievements();
	
	void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}