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
        public DbSet<BotUserChat> BotUserChat { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotUserChat>()
                .HasKey(b => new {b.ChatUsersId, b.UserChatsChatId});
            modelBuilder.Entity<BotUserChat>()
                .HasOne(b => b.Chat)
                .WithMany(c => c.ChatUsers)
                .HasForeignKey(b => b.UserChatsChatId);
            modelBuilder.Entity<BotUserChat>()
                .HasOne(b => b.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(b => b.ChatUsersId);
        }
    }
}
