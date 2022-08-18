using Microsoft.Extensions.Caching.Memory;
using RimionshipServer.Data;
using RimionshipServer.Users;

namespace RimionshipServer.Services
{
    /// <summary>
    /// Handles non-stats related data
    /// </summary>
    public class DataService
    {
        private readonly UserManager userManager;
        private readonly IMemoryCache memoryCache;

        public DataService(UserManager userManager, IMemoryCache memoryCache)
        {
            this.userManager = userManager;
            this.memoryCache = memoryCache;
        }

        public async Task<RimionUser?> GetCachedUserAsync(string playerId)
        {
            string key = GetCacheKey(playerId);

            if (memoryCache.TryGetValue<RimionUser>(playerId, out var user))
                return user;

            user = await userManager.FindByPlayerIdAsync(playerId);
            if (user == null)
                return null;

            using var entry = memoryCache.CreateEntry(key);
            entry.Value = user;
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            entry.SetSlidingExpiration(TimeSpan.FromSeconds(30));

            return user;
        }

        public void InvalidatePlayerCache(string playerId)
            => memoryCache.Remove(GetCacheKey(playerId));

        private string GetCacheKey(string playerId)
            => $"RimionshipServer.Clients.{playerId}";
    }
}
