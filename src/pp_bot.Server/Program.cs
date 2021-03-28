using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace pp_bot.Server
{
    internal static class Program
    {
        private static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    services.AddSingleton<ITelegramBotClient>(
                        new TelegramBotClient(config["BOT_TOKEN"]));
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
