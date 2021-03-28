using System;
using System.Threading;
namespace pp_bot.Server
{
    class Program
    {
        static void Main(string[] args)
        {
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
