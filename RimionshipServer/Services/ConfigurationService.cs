using Flurl;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RimionshipServer.API;
using RimionshipServer.Data;
using System.Security.Cryptography;
using System.Text.Json;

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

        private record LoginToken(string Id, string PlayerId);

        public string GetLoginUrl(string token)
            => options.Value.LoginUrl.SetQueryParam("token", token);
    }
}
