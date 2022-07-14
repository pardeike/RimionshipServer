using Microsoft.EntityFrameworkCore;
using RimionshipServer.Models;

namespace RimionshipServer.Services
{
	public class DataContext : DbContext
	{
		public DbSet<Participant> Participants { get; set; }
		public DbSet<Stat> Stats { get; set; }
		public DbSet<FutureEvent> FutureEvents { get; set; }
		public DbSet<AllowedMod> AllowedMods { get; set; }

		public static readonly string DbPath = "/data/rimionship.db";

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
#if DEBUG
			optionsBuilder.UseSqlite($"Data Source=C:\\Users\\andre\\Documents\\Rimionship\\rimionship.db");
#else
			optionsBuilder.UseSqlite($"Data Source={DbPath}");
#endif
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			_ = modelBuilder.Entity<Stat>()
				 .Property(b => b.Created)
				 .HasDefaultValueSql("getdate()");
		}

		// DEFAULT VALUES
		//
		public async void CreateDefaults()
		{
			var all = await AllowedMods.ToListAsync();
			if (all.Count == 0)
			{
				_ = await AllowedMods.AddAsync(new AllowedMod(1, "brrainz.harmony", 2009463077));
				_ = await AllowedMods.AddAsync(new AllowedMod(2, "ludeon.rimworld", 0));
				_ = await AllowedMods.AddAsync(new AllowedMod(3, "brrainz.rimionship", 2834585352));
				_ = await AllowedMods.AddAsync(new AllowedMod(4, "brrainz.cameraplus", 867467808));
				_ = await SaveChangesAsync();
			}
		}
	}
}
