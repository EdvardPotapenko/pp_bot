using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace pp_bot.Data;

public sealed class PpContextFactory : IDesignTimeDbContextFactory<PPContext>
{
    public PPContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PPContext>()
#if DEBUG
            .UseInMemoryDatabase("pp-bot-db");
#else
            .UseNpgsql("Host=localhost;Port=5432;");
#endif
        return new PPContext(builder.Options);
    }
}