using RimionshipServer.Data;
using System.Collections.Immutable;

namespace RimionshipServer.Services
{
    public class EventsService
    {
        private readonly SemaphoreSlim updateSemaphore = new(1);

        public ImmutableList<UserEvent> Events { get; private set; } = ImmutableList<UserEvent>.Empty;

        public async Task AddOrUpdateEventsAsync(string userId, List<UserEvent> events, CancellationToken cancellationToken = default)
        {
            await updateSemaphore.WaitAsync(cancellationToken);

            Events = Events
                .RemoveAll(evt => evt.UserId == userId)
                .AddRange(events)
                .Sort((a, b) => a.Ticks.CompareTo(a.Ticks));

            updateSemaphore.Release();
        }
    }
}
