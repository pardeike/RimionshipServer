using RimionshipServer.Data;
using RimionshipServer.Users;
using System.Collections.Immutable;

namespace RimionshipServer.Services
{
    public class EventsServiceCulling : IHostedService, IDisposable
    {
        private Timer? _timer = null;
        private IServiceProvider Services { get; }

        public EventsServiceCulling(IServiceProvider services)
        {
            this.Services = services;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Cull, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public async void Cull(object? state)
        {
            using var scope = Services.CreateScope();
            var eventsService = scope.ServiceProvider.GetRequiredService<EventsService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();

            var inactiveUserIds = await userManager.GetInactivePlayerIds(TimeSpan.FromMinutes(1));
            eventsService.CullEvents(inactiveUserIds);
        }
    }

    public class EventsService
    {
        private readonly object _lockobj = new();

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

        public void StartCullingTask(UserManager userManager)
        {
            Task.Run(async () =>
            {
                var inactiveUserIds = await userManager.GetInactivePlayerIds(TimeSpan.FromMinutes(1));
                CullEvents(inactiveUserIds);
                await Task.Delay(TimeSpan.FromSeconds(30));
            });
        }

        public void CullEvents(List<string> ids)
        {
            var inactiveUserIds = ids.ToHashSet();
            lock (_lockobj)
            {
                var tmpList = Events.ToList();
                tmpList.RemoveAll(evt => inactiveUserIds.Contains(evt.UserId));
                Events = tmpList.OrderBy(x => x.Ticks)
                                .ToImmutableList();
            }
        }
    }
}
