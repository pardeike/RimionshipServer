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
				_ = await AllowedMods.AddAsync(new AllowedMod(01, "brrainz.harmony", 2009463077));
				_ = await AllowedMods.AddAsync(new AllowedMod(02, "ludeon.rimworld", 0));
				_ = await AllowedMods.AddAsync(new AllowedMod(03, "unlimitedhugs.hugslib", 818773962));
				_ = await AllowedMods.AddAsync(new AllowedMod(04, "brrainz.rimionship", 2834585352));
				_ = await AllowedMods.AddAsync(new AllowedMod(04, "mastertea.randomplus", 1434137894)); // remove later
				_ = await AllowedMods.AddAsync(new AllowedMod(05, "automatic.bionicicons", 1677616980));
				_ = await AllowedMods.AddAsync(new AllowedMod(06, "brrainz.cameraplus", 867467808));
				_ = await AllowedMods.AddAsync(new AllowedMod(06, "jaxe.rimhud", 1508850027));
				_ = await AllowedMods.AddAsync(new AllowedMod(07, "tiagocc0.colorblindminerals", 1424338139));
				_ = await AllowedMods.AddAsync(new AllowedMod(08, "dubwise.dubsmintmenus", 1446523594));
				_ = await AllowedMods.AddAsync(new AllowedMod(09, "dubwise.dubsmintminimap", 1662119905));
				_ = await AllowedMods.AddAsync(new AllowedMod(10, "fluffy.followme", 715759739));
				_ = await AllowedMods.AddAsync(new AllowedMod(11, "falconne.heatmap", 947972722));
				_ = await AllowedMods.AddAsync(new AllowedMod(12, "krafs.levelup", 1701592470));
				_ = await AllowedMods.AddAsync(new AllowedMod(13, "fluffy.medicaltab", 715565817));
				_ = await AllowedMods.AddAsync(new AllowedMod(14, "com.github.alandariva.moreplanning", 2551225702));
				_ = await AllowedMods.AddAsync(new AllowedMod(15, "fluffy.musicmanager", 2229205672));
				_ = await AllowedMods.AddAsync(new AllowedMod(16, "peppsen.pmusic", 725130005));
				_ = await AllowedMods.AddAsync(new AllowedMod(17, "legodude17.qualcolor", 2420141361));
				_ = await AllowedMods.AddAsync(new AllowedMod(19, "automatic.recipeicons", 1616643195));
				_ = await AllowedMods.AddAsync(new AllowedMod(20, "targhetti.showdrafteesweapon", 1690978457));
				_ = await AllowedMods.AddAsync(new AllowedMod(21, "crashm.colorcodedmoodbar.11", 2006605356));
				_ = await AllowedMods.AddAsync(new AllowedMod(22, "mlie.silentdoors", 2012447929));
				_ = await AllowedMods.AddAsync(new AllowedMod(23, "heye.twitchchat", 2075845400));
				_ = await AllowedMods.AddAsync(new AllowedMod(24, "vanillaexpanded.vhe", 1888705256));
				_ = await AllowedMods.AddAsync(new AllowedMod(25, "bodlosh.weaponstats", 974066449));
				_ = await AllowedMods.AddAsync(new AllowedMod(26, "odeum.wmbp", 2314407956));
				_ = await AllowedMods.AddAsync(new AllowedMod(27, "showhair.kv.rw", 1180826364));
				_ = await SaveChangesAsync();
			}
			PlayState.SetString(StateKey.ServerMessage, "");
			PlayState.SetInt(StateKey.StartingPawnCount, 5);
			PlayState.SetString(StateKey.GameFileUrl, "https://mod.rimionship.com/game/rimionship.rws");
			PlayState.SetString(StateKey.GameFileHash, "837e504433e8f5ffa9283271e4906063");
			PlayState.SetEnum(StateKey.GameState, Game.Training);
			PlayState.SetInt(StateKey.GameStartHour, 0);
			PlayState.SetInt(StateKey.GameStartMinute, 0);
			PlayState.SetFloat(StateKey.ScaleFactor, 0.2f);
			PlayState.SetFloat(StateKey.GoodTraitSuppression, 0.7f);
			PlayState.SetFloat(StateKey.BadTraitSuppression, 0.15f);
			PlayState.SetInt(StateKey.MaxFreeColonistCount, 5);
			PlayState.SetInt(StateKey.RisingInterval, 2400000);
			PlayState.SetInt(StateKey.RisingReductionPerColonist, 240000);
			PlayState.SetInt(StateKey.RisingIntervalMinimum, 120000);
			PlayState.SetInt(StateKey.RisingCooldown, 120000);
			PlayState.SetInt(StateKey.StartPauseInterval, 120000);
			PlayState.SetInt(StateKey.FinalPauseInterval, 120000);
			PlayState.SetFloat(StateKey.MinThoughtFactor, 1f);
			PlayState.SetFloat(StateKey.MaxThoughtFactor, 3f);
		}
	}
}
