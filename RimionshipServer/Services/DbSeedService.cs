using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;

namespace RimionshipServer.Services
{
    /// <summary>
    /// Used to seed the database on initial startup.
    /// An own class to avoid cluttering the DbContext and allow injection of IOptions.
    /// </summary>
    public class DbSeedService
    {
        private readonly RimionDbContext db;

        public DbSeedService(RimionDbContext db)
        {
            this.db = db;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            await SeedAllowedModsAsync(cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedAllowedModsAsync(CancellationToken cancellationToken = default)
        {
            if (await db.AllowedMods.AnyAsync(cancellationToken))
                return;

            // TODO: Add the list here, _probably_ from a configuration we have yet to inject
        }
    }
}
