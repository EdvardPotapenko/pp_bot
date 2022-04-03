using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace pp_bot.Data;

public sealed class PpContextFactory : IDesignTimeDbContextFactory<PP_Context>
{
    public PP_Context CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PP_Context>()
#if DEBUG
            .UseInMemoryDatabase("pp-bot-db");
#else
            .UseNpgsql("Host=localhost;Port=5432;");
#endif
        return new PP_Context(builder.Options);
    }
}