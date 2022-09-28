using Microsoft.AspNetCore.SignalR;
using NUglify.Helpers;
using RimionshipServer.API;
using System.Collections.Immutable;

namespace RimionshipServer.Services
{
    public class DirectionService
    {
        private readonly IHubContext<DashboardHub, IDashboardClient> dashboardContext;
        public ImmutableDictionary<string, string> DirectionInstructions { get; private set; } = ImmutableDictionary<string, string>.Empty;
        private readonly SemaphoreSlim directionSemaphore = new SemaphoreSlim(1);

        public DirectionService(IHubContext<DashboardHub, IDashboardClient> dashboardContext)
        {
            this.dashboardContext = dashboardContext;
        }

        public async Task SetDirectionInstruction(string userId, string? comment)
        {
            await directionSemaphore.WaitAsync();
            try
            {
                if (comment.IsNullOrWhiteSpace())
                    DirectionInstructions = DirectionInstructions.Remove(userId);
                else
                    DirectionInstructions = DirectionInstructions.SetItem(userId, comment!);

                await dashboardContext.Clients.All.SetDirectionInstruction(new DirectionInstruction(userId, comment));
            }
            finally
            {
                directionSemaphore.Release();
            }
        }

    }
}
