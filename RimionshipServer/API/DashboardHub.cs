using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Services;

namespace RimionshipServer.API
{
    public record UserInfo(string Id, string UserName, string? AvatarUrl);

    public interface IDashboardClient
    {
        Task AddUser(UserInfo userInfo);
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

        public async Task<List<UserInfo>> GetUsers()
        {
            return await db.Users
                .Select(u => new UserInfo(u.Id, u.UserName, u.AvatarUrl))
                .ToListAsync();
        }

        public async Task<List<LatestStats>> GetLatestStats()
        {
            return await db.LatestStats
                .AsNoTracking()
                .ToListAsync();
        }

        public record UserAttention(string UserId, long Score);

        public List<UserAttention> GetAttentionList()
        {
            return attentionService.GetAttentionScores()
                .Select(u => new UserAttention(u.Name, u.Score))
                .ToList();
        }
    }
}
