﻿using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
namespace RimionshipServer.Data
{
	public abstract partial class Stats
	{
        static Stats()
        {
            InitStatToUser();
        }

        public static void ClearCache()
        {
            foreach (ConcurrentDictionary<string,double> concurrentDictionary in _StatToUser.Values)
                concurrentDictionary.Clear();
        }
        
        public static async Task<Dictionary<string, Dictionary<string, double>>> GetFinalResults(RimionDbContext context)
        {
            var ret = new Dictionary<string, Dictionary<string, double>>();
            foreach (var statInDb in _StatToUser.Keys)
            {
                var ids = _StatToUser[statInDb].OrderByDescending(x => x.Value)
                                               .Select(x => x.Key)
                                               .ToList();
                var values = _StatToUser[statInDb].OrderByDescending(x => x.Value)
                                                  .Select(x => x.Value)
                                                  .ToList();
                (string UserName, string stat, double value)[] valueTuples = (await context.Users
                                                                                           .Where(l => ids.Contains(l.Id))
                                                                                           .Where(x => !x.WasBanned)
                                                                                           .ToListAsync())
                                                                            .OrderBy(l => ids.IndexOf(l.Id))
                                                                            .Select((x, y) => (x.UserName, statInDb, values[y]))
                                                                            .ToArray();
                foreach (var (userName, stat, value) in valueTuples)
                {
                    if (!ret.TryGetValue(userName, out var innerDict))
                    {
                        ret.Add(userName, new(){{stat, value}});
                    }
                    else
                    {
                        innerDict.Add(stat, value);
                    }
                }
            }
            return ret;
        }
        
        public static async Task<(int, string, double)[]> GetTopXNotBannedUserFromDynamicCacheWithValue(string stat, int max, RimionDbContext context)
        {
            var ids = _StatToUser[stat].OrderByDescending(x => x.Value)
                                       .Select(x => x.Key)
                                       .ToList();
            var values = _StatToUser[stat].OrderByDescending(x => x.Value)
                                       .Select(x => x.Value)
                                       .ToList();
            return (await context.Users
                                 .Where(l => ids.Contains(l.Id))
                                 .Where(x => !x.WasBanned)
                                 .ToListAsync())
                  .OrderBy(l => ids.IndexOf(l.Id))
                  .Take(max)
                  .Select((x, y) => (y, x.UserName, values[y]))
                  .ToArray();
        }

        public static async Task<RimionUser[]> GetTopXNotBannedUserFromDynamicCache(string stat, int max, RimionDbContext context)
        {
             var ids = _StatToUser[stat].OrderByDescending(x => x.Value)
                                    .Select(x => x.Key)
                                    .ToList();
             
             return (await context.Users
                         .Where(l => ids.Contains(l.Id))
                         .Where(x => !x.WasBanned)
                         .ToListAsync())
                         .OrderBy(l => ids.IndexOf(l.Id))
                         .Take(max)
                         .ToArray();
        }

        public string UserId { get; set; } = null!;

		public DateTimeOffset Timestamp { get; set; }

		public virtual RimionUser User                                              { get; set; } = null!;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public         void       UpdateFromRequest(API.StatsRequest         stats) => UpdateFromRequestInternal(stats);

        public static IEnumerable<Task> InitStatFromDatabase(RimionDbContext context)
        {
            return FieldNames.Select(fieldName => context.GetTopXUser(fieldName).AsNoTrackingWithIdentityResolution().ForEachAsync(x => _StatToUser[fieldName][x.UserId] = x.Value));
        }
#region Partial Methodes
        partial        void UpdateFromRequestInternal(API.StatsRequest stats);
        static partial void InitStatToUser();
#endregion
    }
}
