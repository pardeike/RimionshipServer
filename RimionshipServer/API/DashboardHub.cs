﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Services;

namespace RimionshipServer.API
{

    [Authorize(Roles = Roles.Admin)]
    public class DashboardHub : Hub<IDashboardClient>
    {
        private readonly RimionDbContext db;
        private readonly AttentionService attentionService;
        private readonly DirectionService directionService;
        private readonly EventsService eventsService;
        public Serilog.ILogger logger = Serilog.Log.ForContext<DashboardHub>();

        public DashboardHub(
            RimionDbContext db,
            AttentionService attentionService,
            DirectionService dataService,
            EventsService eventsService)
        {
            this.db = db;
            this.attentionService = attentionService;
            this.directionService = dataService;
            this.eventsService = eventsService;
        }

        public async Task<List<UserInfo>> GetUsers()
        {
            return await db.Users
                .AsNoTrackingWithIdentityResolution()
                .Select(u => new UserInfo(u.Id, u.UserName, u.WasBanned, u.HasQuit, u.WasShownTimes, u.AvatarUrl))
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
                .Where(asw => asw.Score > 0)
                .Select(u => new UserAttention(u.Name, u.Score))
                .ToList();
        }

        public void ResetAttentionList()
        {
            attentionService.ResetAttentionList();
        }

        public void ResetAttention(string userId, long score)
        {
            attentionService.ResetAttention(userId, score);
        }

        public int SetAttentionReduction(int delta)
        {
            return attentionService.ChangeDecrement(delta);
        }

        public List<DirectionInstruction> GetDirectionInstructions()
            => directionService.DirectionInstructions
                .Select(p => new DirectionInstruction(p.Key, p.Value))
                .ToList();

        public async Task SetDirectionInstruction(string userId, string? comment)
            => await directionService.SetDirectionInstruction(userId, comment);

        public List<UserEvent> GetEventsList()
        {
            return eventsService.Events.ToList();
        }

        public async void SwitchTwitchChannel(string userName)
        {
            await db.SetStreamerAsync(userName, true);
            Console.WriteLine($"CAST ------> {userName}");
        }
    }
}
