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
        private readonly RimionDbContext db;
        private readonly IUserStore userStore;
        private readonly IMemoryCache memoryCache;

        public DataService(RimionDbContext db, IUserStore userStore, IMemoryCache memoryCache)
        {
            this.db = db;
            this.userStore = userStore;
            this.memoryCache = memoryCache;
        }

        public async Task<RimionUser?> GetCachedUserAsync(string playerId, CancellationToken cancellationToken = default)
        {
            string key = $"RimionshipServer.Clients.{playerId}";

            if (memoryCache.TryGetValue<RimionUser>(playerId, out var user))
                return user;

            user = await userStore.FindUserByClientIdAsync(playerId, cancellationToken);
            if (user == null)
                return null;

            using var entry = memoryCache.CreateEntry(key);
            entry.Value = user;
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            entry.SetSlidingExpiration(TimeSpan.FromSeconds(30));

            return user;
        }
    }
}
