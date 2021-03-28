using System;
using Telegram.Bot;
using pp_bot.bot.configuration;
using pp_bot.model;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Linq;

namespace pp_bot
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