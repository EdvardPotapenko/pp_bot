using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pp_bot.Server.model;
using pp_bot.Server.Services;
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
                    services.AddSingleton<ITelegramBotClient>(
                        new TelegramBotClient(config["BOT_TOKEN"]));
                    services.AddSingleton<IUpdateHandler, BotHandler>();
                    services.AddHostedService<BotHandlerService>();
                    services.AddDbContext<PP_Context>(options => options
                        .UseNpgsql(config.GetConnectionString("DB_CONN_STR")));
                })
                .Build();
            //await host.RunAsync();
            
            var bot = new BotStarter();
            bot.Start();
            Console.WriteLine("receiving...");
            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
