using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace pp_bot.Services;

public interface IServiceConfiguration
{
	void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}