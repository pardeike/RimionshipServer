using Flurl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RimionshipServer.API;
using RimionshipServer.Data;

namespace RimionshipServer.Services
{
	public class ConfigurationService
	{
		private const string ModListCacheKey = "RimionshipServer.ModList";
		private readonly LoginService loginService;
		private readonly RimionDbContext db;
		private readonly IMemoryCache memoryCache;
		private readonly IOptions<RimionshipOptions> options;

		public ConfigurationService(
			 LoginService loginService,
			 RimionDbContext db,
			 IMemoryCache memoryCache,
			 IOptions<RimionshipOptions> options)
		{
			this.loginService = loginService;
			this.db = db;
			this.memoryCache = memoryCache;
			this.options = options;
		}

        public async Task EditModOrder(byte modOrder, byte originalLoadOrder)
        {
            var newPlace = await db.AllowedMods.FirstAsync(x => x.LoadOrder == originalLoadOrder);
            newPlace.LoadOrder = modOrder;
            var oldPlace = await db.AllowedMods.FirstAsync(x => x.LoadOrder == modOrder);
            oldPlace.LoadOrder = originalLoadOrder;
            db.AllowedMods.Update(newPlace);
            db.AllowedMods.Update(oldPlace);
            await SaveChanges();
        }
        
        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
            FlushAllowedModsAsync();
        }

        public async Task AddAllowedMod(AllowedMod mod)
        {
            db.AllowedMods.Add(mod);
            await SaveChanges();
        }
        
        public async Task RemoveAllowedModAsync(string PackageId, ulong SteamId)
        {
            var mod = await db.AllowedMods.FirstAsync(x => x.PackageId == PackageId && x.SteamId == SteamId);
            db.AllowedMods.Remove(mod);
            await db.AllowedMods
                    .Where(x => x.LoadOrder > mod.LoadOrder)
                    .ForEachAsync(x => x.LoadOrder--);
            await SaveChanges();
        }

        public IQueryable<AllowedMod> GetAllowedModsWithOrderAsync()
        {
            return db.AllowedMods.AsNoTracking();
        }

        /// <summary>
		/// Returns (and caches for a bit) the list of allowed mods
		/// </summary>
		public async Task<IEnumerable<Mod>> GetAllowedModsAsync()
		{
			return await memoryCache.GetOrCreateAsync(ModListCacheKey, async entry =>
			{
				entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
				return await db.AllowedMods.AsNoTracking()
                                       .OrderBy(x => x.LoadOrder)
                                       .Select(m => new Mod { SteamId = m.SteamId, PackageId = m.PackageId })
                                       .ToListAsync();
			});
		}

		/// <summary>
		/// Removes the cache for the mod list, forcing a re-fetching from the database the next time it's requested
		/// </summary>
		public void FlushAllowedModsAsync()
			 => memoryCache.Remove(ModListCacheKey);

        private record LoginToken(string Id, string PlayerId);

		public string GetLoginUrl(string token)
			 => options.Value.LoginUrl.SetQueryParam("token", token);
	}
}
