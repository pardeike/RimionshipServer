using System.Data;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.API;
namespace RimionshipServer.Data
{
	public class RimionDbContext : IdentityDbContext<RimionUser>
	{
		public DbSet<AllowedMod>   AllowedMods  { get; set; } = null!;
		public DbSet<LatestStats>  LatestStats  { get; set; } = null!;
		public DbSet<HistoryStats> HistoryStats { get; set; } = null!;
        // public DbSet<GraphData>    GraphData    { get; set; } = null!;
        
        private DbSet<MiscSettings.BroadcastMessage> MotdSet      { get; set; } = null!;
        
        public async Task SeedMotd()
        {
            if (await MotdSet.AnyAsync())
                return;
            MotdSet.Add(new MiscSettings.BroadcastMessage{Text = String.Empty});
        }
        
        public async Task SetMotdAsync(string? newText)
        {
            var toUpdate = await MotdSet.FirstAsync();
            toUpdate.Text = newText ?? string.Empty;
            MotdSet.Update(toUpdate);
            await SaveChangesAsync();
        }
        
        public async Task<string> GetMotdAsync(CancellationToken cancellationToken = default)
        {
            return (await MotdSet.FirstAsync()).Text;
        }
        
        public DbSet<MiscSettings.Settings> Settings { get; set; } = null!;
		public RimionDbContext(DbContextOptions<RimionDbContext> options)
			 : base(options)
		{
			this.ChangeTracker.LazyLoadingEnabled = false;
		}

		public async Task AddOrUpdateStatsAsync(RimionUser user, StatsRequest request)
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

		public async Task<List<(float Timestamp, object[] Values)>> FetchDataAsync(DateTimeOffset startTime, DateTimeOffset endTime, int intervalSeconds, ICollection<string> columns, string userId)
		{
			if (columns.Except(Stats.FieldNames).Any())
				throw new ArgumentException("Unknown fields in list", nameof(columns));
			if (intervalSeconds <= 0)
				throw new ArgumentOutOfRangeException(nameof(intervalSeconds));

			var bucketString = $"Timestamp / {intervalSeconds * 10_000_000}";
			var sb = new StringBuilder();

			sb.AppendLine(@$"
SELECT DISTINCT 
    {bucketString} AS TimestampBucket,");

			sb.Append(string.Join(",\n", columns.Select(column => $"LAST_VALUE({column}) OVER (PARTITION BY {bucketString}) AS {column}")));

			sb.AppendLine(@"
FROM HistoryStats
WHERE Timestamp >= @startTime AND Timestamp <= @endTime AND UserId = @userId
ORDER BY TimestampBucket ASC, UserId ASC");

			using var cmd = this.Database.GetDbConnection().CreateCommand();
			await Database.OpenConnectionAsync();
			cmd.CommandText = sb.ToString();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqliteParameter("startTime", startTime.UtcTicks));
			cmd.Parameters.Add(new SqliteParameter("endTime", endTime.Ticks));
			cmd.Parameters.Add(new SqliteParameter("userId", userId));
			var reader = await cmd.ExecuteReaderAsync();
			List<(float Timestamp, object[] Values)> result = new();
			while (await reader.ReadAsync())
				result.Add((Timestamp: reader.GetInt64(0), Values: Enumerable.Range(1, reader.FieldCount - 1).Select(reader.GetValue).ToArray()));

			return result;
		}

        private const int TicksPerSecond      = 10_000_000;

        public async Task<(List<DateTimeOffset> Timestamp, List<object> Values)> FetchDataVerticalAsync(DateTimeOffset startTime, DateTimeOffset endTime, int intervalSeconds, string column, string userId)
        {
            if (!Stats.FieldNames.Contains(column))
                throw new ArgumentException("Unknown field", nameof(column));
            if (intervalSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervalSeconds));

            var bucketDivisor = intervalSeconds * TicksPerSecond;
            
            await using var cmd          = Database
                                                      .GetDbConnection()
                                                      .CreateCommand();

            await Database.OpenConnectionAsync();

            cmd.CommandText = 
@$"SELECT (CAST((Timestamp / @bucketDivisor) as int) * @bucketDivisor) as tme, avg({column}) as Val
FROM HistoryStats
WHERE Timestamp >= @startTime AND Timestamp <= @endTime AND UserId = @userId
GROUP BY (Timestamp / @bucketDivisor)";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddRange(new []{
                                              new SqliteParameter("startTime",     startTime.UtcTicks),
                                              new SqliteParameter("endTime",       endTime.UtcTicks),
                                              new SqliteParameter("userId",        userId),
                                              new SqliteParameter("bucketDivisor", bucketDivisor)
                                          });
           
            var reader     = await cmd.ExecuteReaderAsync();
            var timestamps = new List<DateTimeOffset>();
            var values     = new List<object>();

            while (await reader.ReadAsync())
            {
                var timeValue   = reader.GetInt64(0);
                var objectValue = reader.GetValue(1);
                timestamps.Add(new DateTimeOffset(timeValue, TimeSpan.Zero));
                values.Add(objectValue);
            }
            
            return (timestamps, values);
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

            builder.Entity<MiscSettings.BroadcastMessage>(entity =>
                                                          {
                                                              entity.HasKey(x => x.Id);
                                                              entity.ToTable(nameof(MiscSettings.BroadcastMessage));
                                                          });
            builder.Entity<MiscSettings.Settings>(entity =>
                                                  {
                                                      entity.HasIndex(x => x.Name).IsUnique();
                                                      entity.Property(x => x.Id)
                                                            .ValueGeneratedOnAdd()
                                                            .HasAnnotation("Sqlite", "Autoincrement");
                                                      entity.HasKey(x => x.Id);
                                                      entity.HasOne(x => x.Punishment)
                                                            .WithOne()
                                                            .HasForeignKey<MiscSettings.Punishment>("fk_Settings");
                                                      entity.HasOne(x => x.Rising)
                                                            .WithOne()
                                                            .HasForeignKey<MiscSettings.Rising>("fk_Settings");
                                                      entity.HasOne(x => x.Traits)
                                                            .WithOne()
                                                            .HasForeignKey<MiscSettings.Traits>("fk_Settings");
                                                  });
            builder.Entity<MiscSettings.Punishment>(entity =>
                                                    {
                                                        entity.Property(x => x.Id)
                                                              .ValueGeneratedOnAdd()
                                                              .HasAnnotation("Sqlite", "Autoincrement");
                                                        entity.HasKey(x => x.Id);
                                                    });
            builder.Entity<MiscSettings.Rising>(entity =>
                                                {
                                                    entity.Property(x => x.Id)
                                                          .ValueGeneratedOnAdd()
                                                          .HasAnnotation("Sqlite", "Autoincrement");
                                                    entity.HasKey(x => x.Id);
                                                });
            builder.Entity<MiscSettings.Traits>(entity =>
                                                {
                                                    entity.Property(x => x.Id)
                                                          .ValueGeneratedOnAdd()
                                                          .HasAnnotation("Sqlite", "Autoincrement");
                                                    entity.HasKey(x => x.Id);
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