using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Services;

namespace RimionshipServer.API
{
    public interface IDashboardClient
    {
    }

    [Authorize(Roles = Roles.Admin)]
    public class DashboardHub : Hub<IDashboardClient>
    {
        private readonly RimionDbContext db;
        private readonly AttentionService attentionService;
        public Serilog.ILogger logger = Serilog.Log.ForContext<DashboardHub>();

        public DashboardHub(RimionDbContext db, AttentionService attentionService)
        {
            this.db = db;
            this.attentionService = attentionService;
        }

        public class LatestSignalRStats : Stats
        {
            public string UserName { get; set; } = null!;

            public LatestSignalRStats()
            {
            }

            public LatestSignalRStats(Stats stats, string userName)
                : base(stats)
            {
                this.UserName = userName;
            }
        }

        public async Task<List<LatestSignalRStats>> GetLatestStats()
        {
            var result = await db.LatestStats
                .AsNoTracking()
                .Select(u => new { UserName = u.User.UserName, Stats = u })
                .ToListAsync();

            return result.Select(p => new LatestSignalRStats(p.Stats, p.UserName)).ToList();
        }

        public record UserAttention(string UserId, string UserName, long Score, bool Sticky, string Comment);

        public async Task<List<UserAttention>> GetAttentionList()
        {
            return new List<UserAttention>();
        }
    }
}
