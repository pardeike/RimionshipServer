using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.API;
using System.Data;
using System.Text;
namespace RimionshipServer.Data
{
    public class RimionDbContext : IdentityDbContext<RimionUser>
    {
        public DbSet<AllowedMod>        AllowedMods       { get; set; } = null!;
        public DbSet<LatestStats>       LatestStats       { get; set; } = null!;
        public DbSet<HistoryStats>      HistoryStats      { get; set; } = null!;
        public DbSet<GraphData>         GraphData         { get; set; } = null!;
        public DbSet<GraphUser>         GraphUsers        { get; set; } = null!;
        public DbSet<GraphRotationData> GraphRotationData { get; set; } = null!;
        
        public DbSet<SaveFile> SaveFiles { get; set; } = null!;

        private DbSet<MiscSettings.State> GameState { get; set; } = null!;
        private DbSet<MiscSettings.BroadcastMessage> MotdSet { get; set; } = null!;

        private DbSet<MiscSettings.SaveSettings> SaveSettings { get; set; } = null!;

        private static MiscSettings.State? _cachedGameState;
        private static string? _cachedMotd;
        private static string? _cachedStreamer;
        private static MiscSettings.SaveSettings _cachedSaveSettings;

        public async Task SetSaveSettingsAsync(string downloadURI, SaveFile? saveFile, int countColonist)
        {
            var toUpdate = await SaveSettings.FirstAsync();
            toUpdate.SaveFile = saveFile?.Name;
            toUpdate.CountColonists = countColonist;
            toUpdate.DownloadURI = downloadURI;
            SaveSettings.Update(toUpdate);
            await SaveChangesAsync();

            _cachedSaveSettings = await SaveSettings.AsNoTracking().FirstAsync();
        }

        public async Task<MiscSettings.SaveSettings> GetSaveSettingsAsync(CancellationToken cancellationToken = default)
        {
            return _cachedSaveSettings ??= await SaveSettings.AsNoTracking().FirstAsync(cancellationToken);
        }

        public async Task SetGameStateAsync(int gameState, int hour, int minute)
        {
            var toUpdate = await GameState.FirstAsync();
            toUpdate.GameState = gameState;
            toUpdate.PlannedStartHour = hour;
            toUpdate.PlannedStartMinute = minute;
            GameState.Update(toUpdate);
            await SaveChangesAsync();
            _cachedGameState = await GameState.AsNoTracking().FirstAsync();
        }

        public async Task<MiscSettings.State> GetGameStateAsync(CancellationToken cancellationToken = default)
        {
            return _cachedGameState ??= await GameState.AsNoTracking().FirstAsync(cancellationToken);
        }

        public async Task SeedSingeRecordTables()
        {
            if ((await MotdSet.CountAsync()) != 2)
            {
                await Database.OpenConnectionAsync();
                await Database.BeginTransactionAsync();
                await Database.ExecuteSqlRawAsync("DELETE FROM BroadcastMessage WHERE TRUE");
                await Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name == 'BroadcastMessage'");
                await Database.CommitTransactionAsync();
                await Database.CloseConnectionAsync();

                MotdSet.Add(new MiscSettings.BroadcastMessage { Text = String.Empty });
                MotdSet.Add(new MiscSettings.BroadcastMessage { Text = String.Empty });
            }
            if (!await GameState.AnyAsync())
            {
                GameState.Add(new MiscSettings.State
                {
                    GameState = 0,
                    PlannedStartHour = 0,
                    PlannedStartMinute = 0
                });
            }
            if (!await SaveSettings.AnyAsync())
            {
                SaveSettings.Add(new MiscSettings.SaveSettings
                {
                    DownloadURI = String.Empty,
                    CountColonists = 5,
                    SaveFile = null
                });
            }
        }

        public async Task SetMotdAsync(string? newText)
        {
            var toUpdate = await MotdSet.FirstAsync();
            toUpdate.Text = newText ?? string.Empty;
            MotdSet.Update(toUpdate);
            await SaveChangesAsync();
            _cachedMotd = newText;
        }

        public async Task<string> GetMotdAsync(CancellationToken cancellationToken = default)
        {
            return _cachedMotd ??= (await MotdSet.AsNoTracking().FirstAsync(cancellationToken)).Text;
        }

        public async Task SetStreamerAsync(string id, bool useName = false)
        {
            var user = useName
                ? await Users.FirstAsync(x => x.UserName == id)
                : await Users.FirstAsync(x => x.Id == id);
            user.WasShownTimes++;
            Users.Update(user);

            var toUpdate = await MotdSet.FirstAsync(x => x.Id == 2);
            toUpdate.Text = user.UserName ?? string.Empty;
            MotdSet.Update(toUpdate);

            await SaveChangesAsync();

            _cachedStreamer = user.UserName;
        }

        public async Task<string> GetStreamerAsync(CancellationToken cancellationToken = default)
        {
            return _cachedStreamer ??= (await MotdSet.AsNoTracking().FirstAsync(x => x.Id == 2, cancellationToken)).Text;
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

            await using var cmd = this.Database.GetDbConnection().CreateCommand();
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

        private const int TicksPerSecond = 10_000_000;

        public async Task<(List<DateTimeOffset> Timestamp, List<object> Values)> FetchDataVerticalAsync(DateTimeOffset startTime, DateTimeOffset endTime, int intervalSeconds, string column, string userId)
        {
            if (!Stats.FieldNames.Contains(column))
                throw new ArgumentException("Unknown field", nameof(column));
            if (intervalSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervalSeconds));

            var bucketDivisor = intervalSeconds * TicksPerSecond;

            await using var cmd = Database
                                                      .GetDbConnection()
                                                      .CreateCommand();

            await Database.OpenConnectionAsync();

            cmd.CommandText =
@$"SELECT (CAST((Timestamp / @bucketDivisor) as int) * @bucketDivisor) as tme, avg({column}) as Val
FROM HistoryStats
WHERE Timestamp >= @startTime AND Timestamp <= @endTime AND UserId = @userId
GROUP BY (Timestamp / @bucketDivisor)";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddRange(new[]{
                                              new SqliteParameter("startTime",     startTime.UtcTicks),
                                              new SqliteParameter("endTime",       endTime.UtcTicks),
                                              new SqliteParameter("userId",        userId),
                                              new SqliteParameter("bucketDivisor", bucketDivisor)
                                          });

            var reader = await cmd.ExecuteReaderAsync();
            var timestamps = new List<DateTimeOffset>();
            var values = new List<object>();

            while (await reader.ReadAsync())
            {
                var timeValue = reader.GetInt64(0);
                var objectValue = reader.GetValue(1);
                timestamps.Add(new DateTimeOffset(timeValue, TimeSpan.Zero));
                values.Add(objectValue);
            }

            return (timestamps, values);
        }

        public IQueryable<GraphUser> GetTopXUser(string stat)
            => GraphUsers.FromSqlRaw(
         @$"            SELECT UserId, UserName, Value
            FROM (
                SELECT UserId, Value FROM (
                    SELECT UserId, Value FROM (
                        SELECT UserId, {stat} as Value FROM HistoryStats ORDER BY Timestamp DESC
                    ) GROUP BY UserId
                ) ORDER BY Value DESC
            )
            INNER JOIN AspNetUsers ON UserId = Id
            WHERE NOT WasBanned
            ");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GraphUser>(typeBuilder =>
                                      {
                                          typeBuilder.HasNoKey();
                                      });
            builder.Entity<HistoryStats>(typeBuilder =>
                                         {
                                             typeBuilder.HasIndex(x => x.Timestamp);
                                         });

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

            builder.Entity<GraphData>(entity =>
                                      {
                                          entity.Property(x => x.Id)
                                              .ValueGeneratedOnAdd()
                                              .HasAnnotation("Sqlite", "Autoincrement");
                                          entity.HasKey(x => x.Id);
                                      });
            builder.Entity<MiscSettings.SaveSettings>(
                                                      entity =>
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