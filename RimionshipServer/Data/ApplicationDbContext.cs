using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RimionshipServer.Data
{
    public class RimionDbContext : IdentityDbContext<RimionUser>
    {
        public DbSet<AllowedMod> AllowedMods { get; set; } = null!;

        public RimionDbContext(DbContextOptions<RimionDbContext> options)
            : base(options)
        {
        }
    }
}