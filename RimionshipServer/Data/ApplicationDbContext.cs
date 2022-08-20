using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RimionshipServer.Data
{
    public class RimionDbContext : IdentityDbContext<RimionUser>
    {
        public DbSet<AllowedMod> AllowedMods { get; set; } = null!;
        public DbSet<LatestStats> LatestStats { get; set; } = null!;
        public DbSet<HistoryStats> HistoryStats { get; set; } = null!;

        public RimionDbContext(DbContextOptions<RimionDbContext> options)
            : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public async Task AddOrUpdateStatsAsync(RimionUser user, API.StatsRequest request)
        {
            var latestStats = await LatestStats.FindAsync(user.Id);

            if (latestStats == null)
            {
                latestStats = new LatestStats
                {
                    UserId = user.Id
                };

                LatestStats.Add(latestStats);
            }

            latestStats.UpdateFromRequest(request);
            latestStats.Timestamp = DateTimeOffset.UtcNow;

            var historyStats = new HistoryStats
            {
                UserId = user.Id,
                Timestamp = latestStats.Timestamp
            };

            historyStats.UpdateFromRequest(request);
            Add(historyStats);

            await SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AllowedMod>(entity =>
            {
                entity.HasIndex(e => e.SteamId).IsUnique();
                entity.HasIndex(e => e.PackageId).IsUnique();
            });

            builder.Entity<LatestStats>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasOne(e => e.User)
                    .WithOne(e => e.LatestStats)
                    .HasForeignKey<LatestStats>(u => u.UserId);
            });

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset)
                        || p.PropertyType == typeof(DateTimeOffset?));

                foreach (var property in properties)
                {
                    builder.Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetConverter());
                }
            }
        }
    }
}