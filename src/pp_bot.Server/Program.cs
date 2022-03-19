using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using pp_bot.Server.Achievements;
using pp_bot.Server.Helpers;
using pp_bot.Server.Models;
using pp_bot.Server.Services;
using pp_bot.Server.Сommands;
using Serilog;
using Serilog.Sinks.Loki;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
// ReSharper disable MethodHasAsyncOverload

namespace pp_bot.Server
{
    internal static class Program
    {
        private static async Task Main()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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

                    baseType = typeof(IAchievable);
                    foreach (var achievementType in baseType.Assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t.IsClass && t.IsPublic && !t.IsAbstract))
                    {
                        services.AddScoped(baseType, achievementType);
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
                        
                        var credentials = new NoAuthCredentials("http://localhost:3100");
                        var lokiLogger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .Enrich.FromLogContext()
                            .WriteTo.LokiHttp(credentials)
                            .CreateLogger();
                        builder.AddSerilog(lokiLogger);
                    }
                })
                .Build();


            var logger = host.Services.GetRequiredService<ILogger<DatabaseSeeder>>();

            try
            {

                using (var scope = host.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PP_Context>();

                    if (context.Database.GetPendingMigrations().Any())
                        context.Database.Migrate();

                    var achievements = scope.ServiceProvider.GetServices<IAchievable>();

                    await DatabaseSeeder.EnsureAchievementsIntegrity(achievements, context);
                }

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Exception occurred while applying migrations or seeding database");
                return;
            }
            
            await host.RunAsync();
        }
    }
}
