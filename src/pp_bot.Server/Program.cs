using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using pp_bot.Server.Models;
using pp_bot.Server.Services;
using pp_bot.Server.Сommands;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
// ReSharper disable MethodHasAsyncOverload

namespace pp_bot.Server
{
    internal static class Program
    {
        private static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddEnvironmentVariables("ASPNETCORE_");
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment.EnvironmentName;
                    builder
                        .AddJsonFile("botsettings.json", false, false)
                        .AddJsonFile($"botsettings.{env}.json", true, false)
                        .AddJsonFile("dbsettings.json", false, false)
                        .AddJsonFile($"dbsettings.{env}.json", true, false)
                        .AddJsonFile("sentrysettings.json", false, false)
                        .AddJsonFile($"sentrysettings.{env}.json", true, false);
                })
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    services.AddLogging();
                    services.AddSingleton<CommandPatternManager>();
                    services.AddSingleton<IAchievementManager, AchievementManager>();
                    services.AddSingleton<ITelegramBotClient>(
                        new TelegramBotClient(config["BOT_TOKEN"]));
                    services.AddSingleton<IUpdateHandler, BotHandler>();
                    services.AddHostedService<BotHandlerService>();
                    services.AddDbContext<PP_Context>(options => options
                        .UseNpgsql(config.GetConnectionString("DB_CONN_STR")));

                    var baseType = typeof(IChatAction);
                    foreach (var commandType in baseType.Assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t.IsClass && t.IsPublic && !t.IsAbstract))
                    {
                        services.AddScoped(baseType, commandType);
                    }
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration);
                    builder.AddConsole();
                    builder.AddDebug();
                    if (context.HostingEnvironment.IsProduction())
                    {
                        builder.AddSentry(context.Configuration["Sentry:Dsn"]);
                    }
                })
                .Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PP_Context>();
                if (context.Database.GetPendingMigrations().Any())
                    context.Database.Migrate();
            }
            
            await host.RunAsync();
        }
    }
}
