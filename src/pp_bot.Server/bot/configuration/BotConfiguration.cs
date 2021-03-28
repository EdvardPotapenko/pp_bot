using System;
using System.Collections.Generic;
using Telegram.Bot.Args;
using Telegram.Bot;
using pp_bot.Server.bot.chat_actions;
using pp_bot.Server.bot.chat_actions.commands;
using pp_bot.Server.model;

namespace pp_bot.Server.bot.configuration
{
    public class BotConfiguration
    {
        private List<IChatAction> _botActions { get; init; }

        private ITelegramBotClient _Client {get;init;}
        private PP_Context _Context{get;set;}

        public BotConfiguration(ITelegramBotClient client, PP_Context context)
        {
            _Client = client;  
            _Context = context;
            _botActions = new List<IChatAction>();        
            this.ConfigureActions();
        }

        public async void OnUpdate(object _sender, UpdateEventArgs _args)
        {
            try
            {
                foreach (var action in _botActions)
                {
                    if (action.Contains(_args.Update.Message))
                        await action.Execute(_args.Update.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// add your IChatAction actions here
        private void ConfigureActions()
        {
            _botActions.Add(new GrowPPCommand(_Client, _Context));
            _botActions.Add(new ShowScoreCommand(_Client, _Context));
            _botActions.Add(new LeaveGameCommand(_Client, _Context));
        }
    }
}