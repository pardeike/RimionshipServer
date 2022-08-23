using Microsoft.EntityFrameworkCore;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Users;

namespace RimionshipServer.Services
{
	/// <summary>
	/// Used to seed the database on initial startup.
	/// An own class to avoid cluttering the DbContext and allow injection of IOptions.
	/// </summary>
	public class DbSeedService
	{
		private readonly RimionDbContext db;
		private readonly IHostEnvironment env;
		private readonly UserManager userManager;
		private readonly RoleManager roleManager;

		public DbSeedService(
			 RimionDbContext db,
			 IHostEnvironment env,
			 UserManager userManager,
			 RoleManager roleManager)
		{
			this.db = db;
			this.env = env;
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		public async Task SeedAsync(CancellationToken cancellationToken = default)
		{
			await SeedUserDataAsync();
			await SeedAllowedModsAsync(cancellationToken);
			await db.SaveChangesAsync(cancellationToken);

			if (env.IsDevelopment())
			{
				if (Environment.GetCommandLineArgs().Contains("--fillMeUp"))
				{
					await CreateBotUsersAsync();
					await PopulateDataAsync();
					await db.SaveChangesAsync(cancellationToken);
					Environment.Exit(0);
					return;
				}
			}
		}

		private async Task SeedUserDataAsync()
		{
			if (!await roleManager.RoleExistsAsync(Roles.Admin))
			{
				var result = await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole
				{
					Name = Roles.Admin
				});

				if (!result.Succeeded)
					throw new Exception($"Cannot seed roles: {string.Join("\n", result.Errors)}");
			}
		}

		private async Task SeedAllowedModsAsync(CancellationToken cancellationToken = default)
		{
			if (await db.AllowedMods.AnyAsync(cancellationToken))
				return;

			db.AllowedMods.AddRange(
				 new AllowedMod { PackageId = "brrainz.harmony", SteamId = 2009463077 },
				 new AllowedMod { PackageId = "ludeon.rimworld", SteamId = 0 },
				 new AllowedMod { PackageId = "unlimitedhugs.hugslib", SteamId = 818773962 },
				 new AllowedMod { PackageId = "brrainz.rimionship", SteamId = 2834585352 },
				 new AllowedMod { PackageId = "mastertea.randomplus", SteamId = 1434137894 }, // remove later
				 new AllowedMod { PackageId = "automatic.bionicicons", SteamId = 1677616980 },
				 new AllowedMod { PackageId = "brrainz.cameraplus", SteamId = 867467808 },
				 new AllowedMod { PackageId = "jaxe.rimhud", SteamId = 1508850027 },
				 new AllowedMod { PackageId = "tiagocc0.colorblindminerals", SteamId = 1424338139 },
				 new AllowedMod { PackageId = "dubwise.dubsmintmenus", SteamId = 1446523594 },
				 new AllowedMod { PackageId = "dubwise.dubsmintminimap", SteamId = 1662119905 },
				 new AllowedMod { PackageId = "fluffy.followme", SteamId = 715759739 },
				 new AllowedMod { PackageId = "falconne.heatmap", SteamId = 947972722 },
				 new AllowedMod { PackageId = "krafs.levelup", SteamId = 1701592470 },
				 new AllowedMod { PackageId = "fluffy.medicaltab", SteamId = 715565817 },
				 new AllowedMod { PackageId = "fluffy.musicmanager", SteamId = 2229205672 },
				 new AllowedMod { PackageId = "peppsen.pmusic", SteamId = 725130005 },
				 new AllowedMod { PackageId = "legodude17.qualcolor", SteamId = 2420141361 },
				 new AllowedMod { PackageId = "automatic.recipeicons", SteamId = 1616643195 },
				 new AllowedMod { PackageId = "targhetti.showdrafteesweapon", SteamId = 1690978457 },
				 new AllowedMod { PackageId = "crashm.colorcodedmoodbar.11", SteamId = 2006605356 },
				 new AllowedMod { PackageId = "mlie.silentdoors", SteamId = 2012447929 },
				 new AllowedMod { PackageId = "heye.twitchchat", SteamId = 2075845400 },
				 new AllowedMod { PackageId = "vanillaexpanded.vhe", SteamId = 1888705256 },
				 new AllowedMod { PackageId = "bodlosh.weaponstats", SteamId = 974066449 },
				 new AllowedMod { PackageId = "odeum.wmbp", SteamId = 2314407956 },
				 new AllowedMod { PackageId = "com.github.alandariva.moreplanning", SteamId = 2551225702 },
				 new AllowedMod { PackageId = "showhair.kv.rw", SteamId = 1180826364 });
		}


		private IEnumerable<string> GetBotIds() => Enumerable.Range(0, 100).Select(i => $"BOT-{i:000}");

		private async Task CreateBotUsersAsync()
		{
			foreach (var id in GetBotIds())
			{
				if (await userManager.FindByIdAsync(id) == null)
				{
					var user = new RimionUser
					{
						Id = id,
						UserName = id,
						AvatarUrl = "http://localhost/404.png",
					};

					var result = await userManager.CreateAsync(user);
					if (!result.Succeeded)
						throw new NotImplementedException();

					await userManager.AddPlayerIdAsync(user, id);
				}
			}
		}

		private async Task PopulateDataAsync()
		{
			await db.Database.ExecuteSqlRawAsync("DELETE FROM HistoryStats");

			var duration = TimeSpan.FromHours(11);
			var interval = TimeSpan.FromSeconds(10);
			var end = DateTimeOffset.UtcNow;
			var rng = new Random();
			foreach (var id in GetBotIds())
				CreateTestDataAsync(
					 id,
					 end.Subtract(duration).Add(TimeSpan.FromSeconds(rng.Next(0, 3600))),
					 end.Subtract(TimeSpan.FromSeconds(rng.Next(0, 360))),
					 interval);
		}

		private void CreateTestDataAsync(string userId, DateTimeOffset start, DateTimeOffset end, TimeSpan interval)
		{
			var rng = new Random();

			var resetCounter = -rng.Next();
			var stats = new StatsRequest();
			var props = stats.GetType().GetProperties().Where(c => c.PropertyType == typeof(int) || c.PropertyType == typeof(float)).ToList();

			for (var now = start; now < end; now += interval + TimeSpan.FromSeconds(rng.NextDouble() - 0.5))
			{
				resetCounter += rng.Next(0, 100);
				if (resetCounter > 0)
				{
					stats = new StatsRequest();
					resetCounter = -rng.Next();
				}

				foreach (var prop in props)
				{
					var val = prop.GetValue(stats);
					prop.SetValue(stats, val switch
					{
						float f => (object)(float)(f + (rng.NextDouble() - 0.4) * 10),
						int i => (object)(int)(i + rng.Next(-100, 100)),
						_ => throw new Exception()
					});
				}

				var historyStats = new HistoryStats
				{
					UserId = userId,
					Timestamp = now
				};

				historyStats.UpdateFromRequest(stats);
				db.HistoryStats.Add(historyStats);
			}
		}
	}
}
