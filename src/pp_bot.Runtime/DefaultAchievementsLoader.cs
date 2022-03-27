using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Achievements;
using pp_bot.Runtime.Composition;
using pp_bot.Services;

namespace pp_bot.Runtime;

internal sealed class DefaultAchievementsLoader : IAchievementsLoader, IDisposable
{
	private readonly ILogger<DefaultAchievementsLoader> _logger;
	
	public IEnumerable<ExportFactoryDependencyInjection<IAchievable, AchievementMetadata>> AchievableFactory { get; private set; }
	public IEnumerable<ExportFactoryDependencyInjection<ITriggerable, AchievementMetadata>> TriggerableFactory { get; private set; }
	public IEnumerable<ExportFactory<IServiceConfiguration>>? ServicesMetadata { get; private set; }
	
	private readonly IDictionary<string, PluginLoadContext> _loadContexts =
		new Dictionary<string, PluginLoadContext>();

	public DefaultAchievementsLoader(ILogger<DefaultAchievementsLoader> logger)
	{
		_logger = logger;
		AchievableFactory = Array.Empty<ExportFactoryDependencyInjection<IAchievable, AchievementMetadata>>();
		TriggerableFactory = Array.Empty<ExportFactoryDependencyInjection<ITriggerable, AchievementMetadata>>();
	}

	public void LoadAchievements()
	{
		var currentAssembly = Assembly.GetExecutingAssembly();
		string assemblyLocation = Path.GetDirectoryName(currentAssembly.Location)!;
		string achievementsLocation = Path.Combine(assemblyLocation, "Achievements");

		var achievementsDir = new DirectoryInfo(achievementsLocation);
		var assemblies = new List<Assembly>();
		
		foreach (DirectoryInfo achievementDir in achievementsDir.EnumerateDirectories())
		{
			_logger.LogInformation("Browsing achievements at {AchievementPath}", achievementDir.FullName);
			
			var loadContext = new PluginLoadContext(achievementDir.FullName);
			_loadContexts.Add(achievementDir.Name, loadContext);

			FileInfo? pluginFile = achievementDir.EnumerateFiles()
				.FirstOrDefault(f => f.Name.Contains(achievementDir.Name, StringComparison.OrdinalIgnoreCase)
									 && f.Extension == ".dll");
			if (pluginFile == null)
				throw new FileNotFoundException("Achievement file was not found");
				
			Assembly achievementAssembly = loadContext.LoadFromAssemblyPath(pluginFile.FullName);
			assemblies.Add(achievementAssembly);
		}

		var configuration = new ContainerConfiguration()
			.WithAssemblies(assemblies)
			.WithProvider(new ExportFactoryWithMetadataDependencyInjectionDescriptorProvider())
			.WithProvider(new ExportFactoryDependencyInjectionDescriptorProvider());

		using var container = configuration.CreateContainer();
		AchievableFactory = container.GetExports<ExportFactoryDependencyInjection<IAchievable, AchievementMetadata>>();
		TriggerableFactory = container.GetExports<ExportFactoryDependencyInjection<ITriggerable, AchievementMetadata>>();
		ServicesMetadata = container.GetExports<ExportFactory<IServiceConfiguration>>();
	}

	public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		if (ServicesMetadata == null)
			throw new ArgumentException(nameof(ServicesMetadata) + " is null");
		
		foreach (var serviceConfigurationFactory in ServicesMetadata)
		{
			var serviceConfiguration = serviceConfigurationFactory.CreateExport();
			serviceConfiguration.Value.ConfigureServices(services, configuration);
		}
	}

	public void Dispose()
	{
		foreach (var (pluginName, pluginContext) in _loadContexts)
		{
			if (pluginContext.IsCollectible)
				pluginContext.Unload();
			_logger.LogInformation("Plugin {PluginName} was successfully unloaded", pluginName);
		}
	}
}