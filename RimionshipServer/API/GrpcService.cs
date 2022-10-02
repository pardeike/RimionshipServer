using Grpc.Core;
using RimionshipServer.Data;
using RimionshipServer.Services;

namespace RimionshipServer.API
{
    public class GrpcService : API.APIBase
    {
        private const int ApiVersion = 1;
        private readonly RimionDbContext db;
        private readonly ConfigurationService configurationService;
        private readonly ScoreService scoreService;
        private readonly EventsService eventsService;
        private readonly DataService dataService;
        private readonly LoginService loginService;
        private readonly AttentionService _attention;
        private readonly SettingService _settingService;
        private readonly LinkGenerator _linkGenerator;

        public GrpcService(
             RimionDbContext db,
             ConfigurationService configurationService,
             ScoreService scoreService,
             EventsService eventsService,
             DataService dataService,
             LoginService loginService,
             AttentionService attention,
             SettingService settingService,
             LinkGenerator linkGenerator)
        {
            this.db = db;
            this.configurationService = configurationService;
            this.scoreService = scoreService;
            this.eventsService = eventsService;
            this.dataService = dataService;
            this.loginService = loginService;
            _attention = attention;
            _settingService = settingService;
            _linkGenerator = linkGenerator;
        }

        /**
		 * Increases the Attention Score for player X by Y amount
		 */
        public override async Task<AttentionResponse> Attention(AttentionRequest request, ServerCallContext context)
        {
            var user = await GetCachedUserAsync(request.Id);
            if (user.HasQuit || user.WasBanned)
                return new AttentionResponse();

            _attention.IncreaseAttentionScore(user.Id, request.Delta);
            return new AttentionResponse();
        }

        /// <summary>
        /// Handles the initial handshake with the client. A call has three possible outcomes:
        /// 1) An exception in case something went wrong, including invalid parameters
        /// 2) A request to log in, accompanied with a login url and a null twitch name
        /// 3) A successful login, in which case we transmit some data down the line
        /// </summary>
        public override async Task<HelloResponse> Hello(HelloRequest request, ServerCallContext context)
        {
            if (ApiVersion != request.ApiVersion)
                throw new RpcException(new Status(StatusCode.PermissionDenied, $"Server expects API version {ApiVersion}, but client sent {request.ApiVersion}"));

            VerifyId(request.Id);

            var allowedModsTask = configurationService.GetAllowedModsAsync();
            var user = await dataService.GetCachedUserByPlayerIdAsync(request.Id);

            HelloResponse response;
            if (user == null)
            {
                response = new HelloResponse()
                {
                    UserExists = false,
                    HasQuit = false,
                    Position = 0,
                    TwitchName = String.Empty
                };
            }
            else
            {
                var (position, scoreEntries) = scoreService.GetPlayerScoreData(request.Id);
                response = new HelloResponse()
                {
                    UserExists = true,
                    HasQuit = user.HasQuit,
                    TwitchName = user.UserName,
                    Position = user.WasBanned ? -1 : position
                };

                if (!user.WasBanned)
                    response.Score.AddRange(scoreEntries.Select(s => new Score { Position = s.Position, LatestScore = s.Score, TwitchName = s.Name }));
            }

            response.AllowedMods.AddRange(await allowedModsTask);

            return response;
        }

        public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            VerifyId(request.Id);
            var (secret, token) = loginService.CreateLoginToken(request.Id);

            return Task.FromResult(new LoginResponse
            {
                LoginToken = secret,
                LoginUrl = configurationService.GetLoginUrl(token)
            });
        }

        public override async Task<LinkAccountResponse> LinkAccount(LinkAccountRequest request, ServerCallContext context)
        {
            VerifyId(request.Id);
            var name = await loginService.RedeemTokenAsync(request.Id, request.LoginToken, context.CancellationToken);

            return new LinkAccountResponse
            {
                UserExists = name != null,
                TwitchName = name ?? string.Empty
            };
        }

        public override async Task<StartResponse> Start(StartRequest request, ServerCallContext context)
        {
            _ = await GetCachedUserAsync(request.Id);
            // banned or quit users are not blocked here

            var settings = await db.GetSaveSettingsAsync();
            return new StartResponse
            {
                GameFileHash = settings.DownloadURI + _linkGenerator.GetPathByPage("/API/SaveFile", "Hash"),
                GameFileUrl = settings.DownloadURI + _linkGenerator.GetPathByPage("/API/SaveFile", "File"),
                StartingPawnCount = settings.CountColonists,
                Settings = await _settingService.GetActiveSetting(db)
            };
        }

        public override async Task<StopResponse> Stop(StopRequest request, ServerCallContext context)
        {
            var user = await GetCachedUserAsync(request.Id);
            user.HasQuit = true;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            dataService.InvalidatePlayerCache(request.Id);
            return new StopResponse();
        }

        public override async Task<FutureEventsResponse> FutureEvents(FutureEventsRequest request, ServerCallContext context)
        {
            if ((State.Types.Game)(await db.GetGameStateAsync(context.CancellationToken)).GameState
               is not State.Types.Game.Started
              and not State.Types.Game.Training)
                return new FutureEventsResponse();

            var user = await GetCachedUserAsync(request.Id);
            if ((user.HasQuit && request.Event.Any()) || user.WasBanned)
                return new FutureEventsResponse();

            var events = request.Event.Select(e => new UserEvent(user.Id, e.Ticks, e.Name, e.Quest, e.Faction, e.Points, e.Strategy, e.ArrivalMode));
            this.eventsService.AddOrUpdateEvents(user.Id, events);

            return new FutureEventsResponse();
        }

        public override async Task<StatsResponse> Stats(StatsRequest request, ServerCallContext context)
        {
            if ((State.Types.Game)(await db.GetGameStateAsync(context.CancellationToken)).GameState
                is not State.Types.Game.Started
               and not State.Types.Game.Training)
                return new StatsResponse { Interval = 30 };

            var user = await GetCachedUserAsync(request.Id);
            if (user.HasQuit)
                return new StatsResponse { Interval = 30 };

            // PARTIALLY implemented - at least, we keep the scores in-memory
            await this.scoreService.AddOrUpdateScoreAsync(request.Id, user.UserName, user.AvatarUrl, request.Wealth, context.CancellationToken);

            await db.AddOrUpdateStatsAsync(user, request);
            return new StatsResponse { Interval = 30 };
        }

        private static readonly ManualResetEventSlim _mres = new();

        public static Task ToggleResetEvent()
        {
            return Task.Factory.StartNew(() =>
                                         {
                                             _mres.Set();
                                             Thread.Sleep(1000);
                                             _mres.Reset();
                                         }, CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.LowestSingleCore);
        }

        public override async Task<SyncResponse> Sync(SyncRequest request, ServerCallContext context)
        {
            VerifyId(request.Id);
            if (request.WaitForChange)
                await Task.Factory.StartNew(() =>
                                        {
                                            _mres.Wait(context.CancellationToken);
                                        }, context.CancellationToken, TaskCreationOptions.LongRunning, PriorityScheduler.Lowest);

            context.CancellationToken.ThrowIfCancellationRequested();

            return new SyncResponse
            {
                Message = await db.GetMotdAsync(context.CancellationToken),
                State = await db.GetGameStateAsync(context.CancellationToken),
                Settings = await _settingService.GetActiveSetting(db, context.CancellationToken)
            };
        }

        private static void VerifyId(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(id)} is empty"));

            if (!Guid.TryParse(id, out _))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(id)} has an invalid scheme"));
        }

        private async Task<RimionUser> GetCachedUserAsync(string clientId)
        {
            VerifyId(clientId);
            return await dataService.GetCachedUserByPlayerIdAsync(clientId) ?? throw new RpcException(new Status(StatusCode.Unauthenticated, "User not found"));
        }
    }
}
