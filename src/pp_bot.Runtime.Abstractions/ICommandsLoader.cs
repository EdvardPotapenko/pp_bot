using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pp_bot.Commands;
using pp_bot.Runtime.Composition;

namespace pp_bot.Runtime;

public interface ICommandsLoader
{
	IEnumerable<ExportFactoryDependencyInjection<IChatAction>> CommandFactory { get; }
	
	void LoadCommands();
	
	void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}