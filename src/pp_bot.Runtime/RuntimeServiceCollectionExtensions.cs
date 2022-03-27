using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace pp_bot.Runtime;

public static class RuntimeServiceCollectionExtensions
{
	public static IServiceCollection AddRuntimeServices(this IServiceCollection services,
		IServiceProvider loggerServices,
		IConfiguration configuration)
	{
		var achievementsLoaderLogger = loggerServices.GetRequiredService<ILogger<DefaultAchievementsLoader>>();
		IAchievementsLoader achievementsLoader = new DefaultAchievementsLoader(achievementsLoaderLogger);
		achievementsLoader.LoadAchievements();
		achievementsLoader.ConfigureServices(services, configuration);

		var commandsLoaderLogger = loggerServices.GetRequiredService<ILogger<DefaultCommandsLoader>>();
		ICommandsLoader commandsLoader = new DefaultCommandsLoader(commandsLoaderLogger);
		commandsLoader.LoadCommands();
		commandsLoader.ConfigureServices(services, configuration);
		
		services.AddSingleton(achievementsLoader);
		services.AddSingleton(commandsLoader);

		return services;
	}
}