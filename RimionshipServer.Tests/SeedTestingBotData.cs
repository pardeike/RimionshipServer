using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Users;
namespace RimionshipServer.Tests
{
    [TestFixture]
    [Ignore("Tooling class")]
    public class SeedTestingBotData
    {
        private RimionDbContext _db          = null!;
        private UserManager     _userManager = null!;
        
        [SetUp]
        public void SetUp()
        {
            var config = new ConfigurationBuilder().Add(new JsonConfigurationSource{
                                                                                         Path = "appsettings.json",
                                                                                         ReloadOnChange = false
                                                                                     }).Build();
            _db = new RimionDbContext(new DbContextOptionsBuilder<RimionDbContext>().UseSqlite(config.GetConnectionString("DefaultConnection")).Options);
            _userManager = new UserManager(new UserStore(_db),
                                          new OptionsWrapper<IdentityOptions>(new IdentityOptions()),
                                          new PasswordHasher<RimionUser>(),
                                          Array.Empty<UserValidator<RimionUser>>(),
                                          Array.Empty<IPasswordValidator<RimionUser>>(),
                                          new UpperInvariantLookupNormalizer(),
                                          new IdentityErrorDescriber(),
                                          default,
                                          NullLogger<UserManager<RimionUser>>.Instance
                                         );
        }
        
        [Test]
        public async Task FillMeUp()
        {
            var nsw = Stopwatch.StartNew();
            await CreateBotUsersAsync();
            await PopulateDataAsync();
            await _db.SaveChangesAsync();
            Assert.Pass(nsw.Elapsed.ToString());
        }

        private static ImmutableList<string> GetBotIds = Enumerable.Range(0, 100).Select(i => $"Id_BOT-{i:000}").ToImmutableList();
        
        private async Task CreateBotUsersAsync()
		{
			foreach (var id in GetBotIds)
            {
                if (await _userManager.FindByIdAsync(id) != null)
                {
                    continue;
                }
                var user = new RimionUser
                           {
                               Id        = id,
                               UserName  = id.Replace("Id_", "UserName_"),
                               AvatarUrl = null
                           };

                await _userManager.CreateAsync(user);
                await _userManager.AddPlayerIdAsync(user, id);
            }
		}
        
        private async Task PopulateDataAsync()
		{
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM HistoryStats WHERE TRUE");
            _db.HistoryStats.AddRange(CreateMultipleTestDataAsync());
        }
        
        private static IEnumerable<HistoryStats> CreateMultipleTestDataAsync()
        {
            var duration = TimeSpan.FromHours(11);
            var interval = TimeSpan.FromSeconds(10);
            var end      = DateTimeOffset.UtcNow;
            var rng      = new Random();
            return GetBotIds.SelectMany(x => CreateTestDataAsync(
                                                                 x,
                                                                 end.Subtract(duration).Add(TimeSpan.FromSeconds(rng.Next(0, 3600))),
                                                                 end.Subtract(TimeSpan.FromSeconds(rng.Next(0,               360))),
                                                                 interval,
                                                                 rng));
        }
        
        private static readonly List<PropertyInfo> Props = typeof(StatsRequest).GetProperties().Where(c => c.PropertyType == typeof(int) || c.PropertyType == typeof(float)).ToList();
        
        private static IEnumerable<HistoryStats> CreateTestDataAsync(string userId, DateTimeOffset start, DateTimeOffset end, TimeSpan interval, Random rng)
		{
            var                resetCounter = -rng.Next();
			var                stats        = new StatsRequest();
            
            for (var now = start; now < end; now += interval + TimeSpan.FromSeconds(rng.NextDouble() - 0.5))
			{
				resetCounter += rng.Next(0, 100);
				if (resetCounter > 0)
				{
					stats = new StatsRequest();
					resetCounter = -rng.Next();
				}

				foreach (var prop in Props)
                {
                    var val = prop.GetValue(stats);
                    switch (val)
                    {
                        case float f:
                            prop.SetValue(stats, (float) (f + (rng.NextDouble() - 0.4) * 10));
                            break;
                        case int i:
                            prop.SetValue(stats, (i + rng.Next(-100, 100)));
                            break;
                    }
                }
                var historyStats = new HistoryStats
                                   {
                                       UserId = userId,
                                       Timestamp = now
                                   };
                historyStats.UpdateFromRequestNoCache(stats);
                yield return historyStats;
            }
        }
    }
}