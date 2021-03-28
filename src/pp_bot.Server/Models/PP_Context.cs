using Microsoft.EntityFrameworkCore;

namespace pp_bot.Server.Models
{
    public sealed class PP_Context : DbContext
    {
        public PP_Context(DbContextOptions<PP_Context> options) : base(options)
        {
        }
        
        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<Chat> Chats { get; set; }
    }
}
