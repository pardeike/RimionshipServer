using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

			using (var cmd = this.Database.GetDbConnection().CreateCommand())
			{
				await Database.OpenConnectionAsync();
				cmd.CommandText = sb.ToString();
				cmd.CommandType = System.Data.CommandType.Text;

				cmd.Parameters.Add(new SqliteParameter("startTime", startTime.UtcTicks));
				cmd.Parameters.Add(new SqliteParameter("endTime", endTime.Ticks));
				cmd.Parameters.Add(new SqliteParameter("userId", userId));
				var reader = await cmd.ExecuteReaderAsync();
				List<(float Timestamp, object[] Values)> result = new();
				while (await reader.ReadAsync())
					result.Add((Timestamp: reader.GetInt64(0), Values: Enumerable.Range(1, reader.FieldCount - 1).Select(reader.GetValue).ToArray()));

				return result;
			}
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