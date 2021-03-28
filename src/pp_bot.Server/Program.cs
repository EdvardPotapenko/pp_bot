using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pp_bot.Server.Models;
using pp_bot.Server.Services;
using pp_bot.Server.Сommands;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace pp_bot.Server
{
    internal static class Program
    {
        private static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment.EnvironmentName;
                    builder
                        .AddJsonFile("botsettings.json", false, false)
                        .AddJsonFile($"botsettings.{env}.json", true, false)
                        .AddJsonFile("dbsettings.json", false, false)
                        .AddJsonFile($"dbsettings.{env}.json", true, false);
                })
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    services.AddSingleton<CommandPatternManager>();
                    services.AddSingleton<ITelegramBotClient>(
                        new TelegramBotClient(config["BOT_TOKEN"]));
                    services.AddSingleton<IUpdateHandler, BotHandler>();
                    services.AddHostedService<BotHandlerService>();
                    services.AddDbContext<PP_Context>(options => options
                        .UseNpgsql(config.GetConnectionString("DB_CONN_STR")));

                    var baseType = typeof(IChatAction);
                    foreach (var commandType in baseType.Assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t)))
                    {
                        services.AddScoped(baseType, commandType);
                    }
                })
                .Build();
            await host.RunAsync();
        }
    }
}
