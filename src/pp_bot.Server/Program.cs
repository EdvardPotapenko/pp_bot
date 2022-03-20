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
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
// ReSharper disable MethodHasAsyncOverload

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("critical_logs.txt")
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var host = new HostBuilder()
        .ConfigureLogging((ctx, cfg) => cfg.ClearProviders())
        .UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration))
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
                .AddJsonFile($"lokisettings.json", false, false)
                .AddJsonFile($"lokisettings.{env}.json", true, false);
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

            baseType = typeof(ITriggerable);
            foreach (var achievementType in baseType.Assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t.IsClass && t.IsPublic && !t.IsAbstract))
            {
                services.AddScoped(baseType, achievementType);
            }
        })               
        .Build();

    using (var scope = host.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<PP_Context>();

        if (context.Database.GetPendingMigrations().Any())
            context.Database.Migrate();

        var achievements = scope.ServiceProvider.GetServices<IAchievable>();
        var triggerables = scope.ServiceProvider.GetServices<ITriggerable>();


        await DatabaseSeedingHelper.EnsureAchievementsIntegrity(achievements, triggerables, context);
    }

    await host.RunAsync();

}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Fatal exception occured");
}