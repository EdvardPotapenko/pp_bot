using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Commands;
using pp_bot.Runtime.Composition;
using pp_bot.Services;

namespace pp_bot.Runtime;

internal sealed class DefaultCommandsLoader : ICommandsLoader
{
	private readonly ILogger<DefaultCommandsLoader> _logger;
	
	public IEnumerable<ExportFactoryDependencyInjection<IChatAction>> CommandFactory { get; private set; }
	public IEnumerable<ExportFactory<IServiceConfiguration>>? ServicesMetadata { get; private set; }
	
	private readonly IDictionary<string, PluginLoadContext> _loadContexts =
		new Dictionary<string, PluginLoadContext>();

	public DefaultCommandsLoader(ILogger<DefaultCommandsLoader> logger)
	{
		_logger = logger;
		CommandFactory = Array.Empty<ExportFactoryDependencyInjection<IChatAction>>();
	}

	public void LoadCommands()
	{
		var currentAssembly = Assembly.GetExecutingAssembly();
		string assemblyLocation = Path.GetDirectoryName(currentAssembly.Location)!;
		string commandsLocation = Path.Combine(assemblyLocation, "Commands");
		
		if (!Directory.Exists(commandsLocation))
		{
			_logger.LogWarning("Directory {DirLocation} doesn't exist", commandsLocation);
			return;
		}

		var commandsDir = new DirectoryInfo(commandsLocation);
		var assemblies = new List<Assembly>();
		
		foreach (DirectoryInfo commandDir in commandsDir.EnumerateDirectories())
		{
			_logger.LogInformation("Browsing commands at {CommandPath}", commandDir.FullName);
			
			var loadContext = new PluginLoadContext(commandDir.FullName);
			_loadContexts.Add(commandDir.Name, loadContext);

			FileInfo? pluginFile = commandDir.EnumerateFiles()
				.FirstOrDefault(f => f.Name.Contains(commandDir.Name, StringComparison.OrdinalIgnoreCase)
									 && f.Extension == ".dll");
			if (pluginFile == null)
				throw new FileNotFoundException("Command file was not found");
				
			Assembly achievementAssembly = loadContext.LoadFromAssemblyPath(pluginFile.FullName);
			assemblies.Add(achievementAssembly);
		}

		var configuration = new ContainerConfiguration()
			.WithAssemblies(assemblies)
			.WithProvider(new ExportFactoryWithMetadataDependencyInjectionDescriptorProvider())
			.WithProvider(new ExportFactoryDependencyInjectionDescriptorProvider());

		using var container = configuration.CreateContainer();
		CommandFactory = container.GetExports<ExportFactoryDependencyInjection<IChatAction>>();
		ServicesMetadata = container.GetExports<ExportFactory<IServiceConfiguration>>();
	}

	public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		if (ServicesMetadata == null)
		{
			_logger.LogWarning("Services metadata collection is null");
			return;
		}
		
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
