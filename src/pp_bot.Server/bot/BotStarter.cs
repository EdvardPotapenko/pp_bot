using System;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Linq;
using pp_bot.Server.bot.configuration;
using pp_bot.Server.model;

namespace pp_bot.Server
{
    public class BotStarter
    {
        BotConfiguration _BotConfiguration { get; set; }
        ITelegramBotClient _Client { get; set; }

        PP_Context _Context { get; set; }
        public BotStarter()
        {
            this.Init();
        }

        public void Start()
        {
            _Client.OnUpdate += _BotConfiguration.OnUpdate;
            _Client.StartReceiving();
        }

        public void Stop()
        {
            _Client.StopReceiving();
        }

        private void Init()
        {
            _Context = new PP_Context();
            // BOT_KEY / DEV_KEY
            string key = Environment.GetEnvironmentVariable("DEV_KEY");
            Console.WriteLine("key " + key);
            _Client = new TelegramBotClient(key);
            _BotConfiguration = new BotConfiguration(_Client, _Context);
        }
    }
}