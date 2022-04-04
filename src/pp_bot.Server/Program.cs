using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pp_bot.Data;
using pp_bot.Data.Helpers;
using pp_bot.Runtime;
using pp_bot.Server.Services;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder();

string env = builder.Environment.EnvironmentName;
builder.Configuration
	.AddJsonFile("botsettings.json", false, false)
	.AddJsonFile($"botsettings.{env}.json", true, false)
	.AddJsonFile("dbsettings.json", false, false)
	.AddJsonFile($"dbsettings.{env}.json", true, false)
	.AddJsonFile("lokisettings.json", false, false)
	.AddJsonFile($"lokisettings.{env}.json", true, false)
	.AddEnvironmentVariables();

if (builder.Configuration["BOT_TOKEN"]?.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase) ?? true)
	throw new InvalidOperationException("Bot token is null");

builder.Logging.ClearProviders();
builder.Host.UseSerilog((_, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));

#pragma warning disable ASP0000
var loggingServices = builder.Logging.Services.BuildServiceProvider();
#pragma warning restore ASP0000
builder.Services.AddRuntimeServices(loggingServices, builder.Configuration);

builder.Services.AddSingleton<CommandPatternManager>();
builder.Services.AddSingleton<IAchievementManager, AchievementManager>();
builder.Services.AddSingleton<ITelegramBotClient>(
	new TelegramBotClient(builder.Configuration["BOT_TOKEN"]));
builder.Services.AddSingleton<IUpdateHandler, BotHandler>();
builder.Services.AddHostedService<BotHandlerService>();

builder.Services.AddDbContext<PPContext>(options => options
#if DEBUG
	.UseInMemoryDatabase("pp-bot-db"));
#else
	.UseNpgsql(builder.Configuration.GetConnectionString("DB_CONN_STR"),
		npgsql => npgsql.MigrationsAssembly("pp_bot.Data")));
#endif
builder.Services.AddScoped<PPBotRepo>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<PPContext>();

#if !DEBUG
	if ((await context.Database.GetPendingMigrationsAsync()).Any())
		await context.Database.MigrateAsync();
#endif

	var achievements = scope.ServiceProvider.GetRequiredService<IAchievementsLoader>();
	await DatabaseSeedingHelper.EnsureAchievementsIntegrityAsync(achievements, context);
}

await app.RunAsync();