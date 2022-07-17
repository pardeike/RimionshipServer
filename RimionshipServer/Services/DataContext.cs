using Microsoft.EntityFrameworkCore;
using RimionshipServer.Models;
using System;
using static Api.State.Types;

namespace RimionshipServer.Services
{
	public class DataContext : DbContext
	{
		public DbSet<Participant> Participants { get; set; }
		public DbSet<Stat> Stats { get; set; }
		public DbSet<FutureEvent> FutureEvents { get; set; }
		public DbSet<AllowedMod> AllowedMods { get; set; }
		public DbSet<PlayState> PlayStates { get; set; }

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
			PlayState.SetString(StateKey.ServerMessage, "");
			PlayState.SetInt(StateKey.StartingPawnCount, 3);
			PlayState.SetString(StateKey.GameFileUrl, "https://mod.rimionship.com/game/rimionship.rws");
			PlayState.SetString(StateKey.GameFileHash, "9c55a4b50658be8d31c1cc6cbf2587d5");
			PlayState.SetEnum(StateKey.GameState, Game.Training);
			PlayState.SetInt(StateKey.GameStartHour, 0);
			PlayState.SetInt(StateKey.GameStartMinute, 0);
			PlayState.SetFloat(StateKey.ScaleFactor, 0.2f);
			PlayState.SetFloat(StateKey.GoodTraitSuppression, 0.7f);
			PlayState.SetFloat(StateKey.BadTraitSuppression, 0.15f);
			PlayState.SetInt(StateKey.MaxFreeColonistCount, 5);
			PlayState.SetInt(StateKey.RisingInterval, 120000);
			PlayState.SetInt(StateKey.RandomStartPauseMin, 140);
			PlayState.SetInt(StateKey.RandomStartPauseMax, 600);
			PlayState.SetInt(StateKey.StartPauseInterval, 30000);
			PlayState.SetInt(StateKey.FinalPauseInterval, 5000);
		}
	}
}
