using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RimionshipServer.API;
using RimionshipServer.Data;

namespace RimionshipServer.Services
{
    public class ConfigurationService
    {
        private const string ModListCacheKey = "RimionshipServer.ModList";
        private readonly RimionDbContext db;
        private readonly IMemoryCache memoryCache;

        public ConfigurationService(RimionDbContext db, IMemoryCache memoryCache)
        {
            this.db = db;
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// Returns (and caches for a bit) the list of allowed mods
        /// </summary>
        public async Task<IEnumerable<Mod>> GetAllowedModsAsync()
        {
            return await memoryCache.GetOrCreateAsync(ModListCacheKey, async (entry) =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return await db.AllowedMods.AsNoTracking().Select(m => new Mod { SteamId = m.SteamId, PackageId = m.PackageId }).ToListAsync();
            });
        }

        /// <summary>
        /// Removes the cache for the mod list, forcing a re-fetching from the database the next time it's requested
        /// </summary>
        public void FlushAllowedModsAsync()
            => memoryCache.Remove(ModListCacheKey);

        /// <summary>
        /// Returns the login URL for the service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetLoginUrl(string id)
            => "http://localhost/not-yet-implemented/";
    }
}
