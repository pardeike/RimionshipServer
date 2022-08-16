using Grpc.Core;
using RimionshipServer.Data;
using RimionshipServer.Services;
using RimionshipServer.Users;

namespace RimionshipServer.API
{
    public class GrpcService : API.APIBase
    {
        private const int ApiVersion = 1;
        private readonly IUserStore userStore;
        private readonly ConfigurationService configurationService;
        private readonly ScoreService scoreService;
        private readonly DataService dataService;

        public GrpcService(
            IUserStore userStore,
            ConfigurationService configurationService,
            ScoreService scoreService,
            DataService dataService)
        {
            this.userStore = userStore;
            this.configurationService = configurationService;
            this.scoreService = scoreService;
            this.dataService = dataService;
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
            var user = await this.userStore.FindUserByClientIdAsync(request.Id, context.CancellationToken);

            HelloResponse response;
            if (user == null)
            {
                response = new HelloResponse()
                {
                    Found = false,
                    LoginUrl = configurationService.GetLoginUrl(request.Id),
                    Position = 0,
                    TwitchName = null
                };
            }
            else
            {
                var (position, scoreEntries) = scoreService.GetPlayerScoreData(request.Id);
                response = new HelloResponse()
                {
                    Found = true,
                    LoginUrl = null,
                    TwitchName = user.UserName,
                    Position = position
                };

                response.Score.AddRange(scoreEntries.Select(s => new Score { Position = s.Position, LatestScore = s.Score, TwitchName = s.Name }));
            }

            response.AllowedMods.AddRange(await allowedModsTask);

            return response;
        }

        public override async Task<StartResponse> Start(StartRequest request, ServerCallContext context)
        {
            var user = await GetCachedUserAsync(request.Id);
            throw new NotImplementedException();
        }

        public override async Task<FutureEventsResponse> FutureEvents(FutureEventsRequest request, ServerCallContext context)
        {
            var user = await GetCachedUserAsync(request.Id);
            throw new NotImplementedException();
        }

        public override async Task<StatsResponse> Stats(StatsRequest request, ServerCallContext context)
        {
            var ct = context.CancellationToken;
            var user = await GetCachedUserAsync(request.Id, ct);

            // PARTIALLY implemented - at least, we keep the scores in-memory
            await this.scoreService.AddOrUpdateScoreAsync(request.Id, user.UserName, user.AvatarUrl, request.Wealth, ct);

            return new StatsResponse { Interval = 10 };
        }

        public override async Task<SyncResponse> Sync(SyncRequest request, ServerCallContext context)
        {
            var user = await GetCachedUserAsync(request.Id);
            throw new NotImplementedException();
        }

        private void VerifyId(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(id)} is empty"));

            if (!Guid.TryParse(id, out _))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(id)} has an invalid scheme"));
        }

        private async Task<RimionUser> GetCachedUserAsync(string clientId, CancellationToken cancellationToken = default)
        {
            VerifyId(clientId);

            return await dataService.GetCachedUserAsync(clientId, cancellationToken) ?? throw new RpcException(new Status(StatusCode.Unauthenticated, "User not found"));
        }
    }
}
