using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using pp_bot.Data;
using pp_bot.Data.Helpers;
using pp_bot.Runtime;
using pp_bot.Server.Options;
using pp_bot.Server.Services;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

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

builder.Logging.ClearProviders();
builder.Host.UseSerilog((_, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));

IServiceProvider loggingServices = builder.Logging.Services.BuildServiceProvider();
builder.Services.AddRuntimeServices(loggingServices, builder.Configuration);

builder.Services.AddOptions<TelegramBotOptions>()
	.BindConfiguration("TelegramBot")
	.ValidateDataAnnotations();

builder.Services.AddSingleton<CommandPatternManager>();
builder.Services.AddSingleton<IAchievementManager, AchievementManager>();
builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
	string token = provider.GetRequiredService<IOptions<TelegramBotOptions>>().Value.Token;
	return new TelegramBotClient(token);
});
builder.Services.AddSingleton<IUpdateHandler, BotHandler>();

switch (builder.Configuration["TelegramBot:ListenKind"])
{
	case "Polling":
		builder.Services.AddHostedService<BotHandlerService>();
		break;
	
	case "Webhook":
		builder.Services.AddHostedService<BotWebhookService>();
		break;
	
	default:
		throw new Exception("TelegramBot:ListenKind parameter is invalid.");
}

builder.Services.AddDbContext<PPContext>(options => options
#if DEBUG
	.UseInMemoryDatabase("pp-bot-db"));
#else
	.UseNpgsql(builder.Configuration.GetConnectionString("DB_CONN_STR"),
		npgsql => npgsql.MigrationsAssembly("pp_bot.Data")));
#endif
builder.Services.AddScoped<PPBotRepo>();

builder.Services.AddControllers();

WebApplication app = builder.Build();

await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
	var context = scope.ServiceProvider.GetRequiredService<PPContext>();

#if !DEBUG
	if ((await context.Database.GetPendingMigrationsAsync()).Any())
		await context.Database.MigrateAsync();
#endif

	var achievements = scope.ServiceProvider.GetRequiredService<IAchievementsLoader>();
	await DatabaseSeedingHelper.EnsureAchievementsIntegrityAsync(achievements, context);
}

app.UseRouting();

app.MapPost("/webhook/v1", async (IUpdateHandler updateHandler, ITelegramBotClient botClient, HttpContext context,
	CancellationToken cancellationToken) =>
{
	Update? update;
		
	using (var streamReader = new StreamReader(context.Request.Body))
	using (var jsonReader = new JsonTextReader(streamReader))
	{
		var serializer = new JsonSerializer();
		update = serializer.Deserialize<Update>(jsonReader);
	}

	if (update == null)
		return Results.BadRequest();
		
	try
	{
		await updateHandler.HandleUpdateAsync(botClient, update, cancellationToken);
	}
	catch (ApiRequestException ex)
	{
		await updateHandler.HandleErrorAsync(botClient, ex, cancellationToken);
		return Results.StatusCode(500);
	}

	return Results.Ok();
});

await app.RunAsync();