using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace pp_bot.Server.Models
{
    public sealed class PpContextFactory : IDesignTimeDbContextFactory<PP_Context>
    {
        public PP_Context CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PP_Context>()
                .UseNpgsql("Host=localhost;Port=5432;");
            return new PP_Context(builder.Options);
        }
    }
}