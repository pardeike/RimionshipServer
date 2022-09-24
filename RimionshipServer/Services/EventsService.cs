using RimionshipServer.Data;
using System.Collections.Immutable;

namespace RimionshipServer.Services
{
    public class EventsService
    {
        private object _lockobj = new();
        public ImmutableList<UserEvent> Events { get; private set; } = ImmutableList<UserEvent>.Empty;

        public void AddOrUpdateEvents(string userId, IEnumerable<UserEvent> events)
        {
            lock (_lockobj)
            {
                var tmpList = Events.ToList();
                tmpList.RemoveAll(evt => evt.UserId == userId);
                tmpList.AddRange(events);
                Events = tmpList.OrderBy(x => x.Ticks)
                                .ToImmutableList();
            }
        }
    }
}
