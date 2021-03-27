using System;
using System.Threading;
namespace pp_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotStarter();
            bot.Start();
            Console.WriteLine("receiving...");
            while(true){
                Thread.Sleep(100);
            }
        }
    }
}
