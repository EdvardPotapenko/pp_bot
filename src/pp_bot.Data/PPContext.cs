using Microsoft.EntityFrameworkCore;
using pp_bot.Data.Models;
// ReSharper disable InconsistentNaming

namespace pp_bot.Data;

public sealed class PPContext : DbContext
{
    public PPContext(DbContextOptions<PPContext> options) : base(options)
    {
    }
        
    public DbSet<BotUser> BotUsers { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Ref__BotUser__Chat> BotUser__Chat { get; set; }
    public DbSet<Ref__BotUserToChat__Achievement> BotUserToChat__Achievement { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<GrowHistory> GrowHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ref__BotUser__Chat>()
            .HasAlternateKey(@ref => new { chat_id = @ref.ChatId, user_id = @ref.UserId });
        modelBuilder.Entity<Ref__BotUser__Chat>()
            .HasOne(@ref => @ref.Chat)
            .WithMany(c => c.ChatUsers)
            .HasForeignKey(b => b.ChatId);
        modelBuilder.Entity<Ref__BotUser__Chat>()
            .HasOne(@ref => @ref.User)
            .WithMany(u => u.UserChats)
            .HasForeignKey(@ref => @ref.UserId);
        modelBuilder.Entity<Ref__BotUser__Chat>()
            .HasMany(@ref => @ref.GrowHistory)
            .WithOne(gh => gh.User)
            .HasForeignKey(gh => gh.Id);

        modelBuilder.Entity<Ref__BotUserToChat__Achievement>()
            .HasAlternateKey(@ref => new { achievement_id = @ref.AchievementId, chat_user_id = @ref.ChatUserId });
        modelBuilder.Entity<Ref__BotUserToChat__Achievement>()
            .HasOne(@ref => @ref.Achievement)
            .WithMany(achievement => achievement.UsersAcquired)
            .HasForeignKey(@ref => @ref.AchievementId)
            .IsRequired();
        modelBuilder.Entity<Ref__BotUserToChat__Achievement>()
            .HasOne(@ref => @ref.ChatUser)
            .WithMany(chatUser => chatUser.AcquiredAchievements)
            .HasForeignKey(@ref => @ref.ChatUserId)
            .IsRequired();
    }
}