using Microsoft.EntityFrameworkCore;
using System;

namespace pp_bot.model
{
    public class PP_Context : DbContext
    {
        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = Environment.GetEnvironmentVariable("PP_BOT_DB_CONNECTION");
            Console.WriteLine("connection " + connection);
            optionsBuilder.UseNpgsql(connection);
        }
    }
}
