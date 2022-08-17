﻿using Grpc.Core;
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
        private readonly LoginService loginService;

        private readonly GameDataService _gameDataService;

        public GrpcService(
            IUserStore userStore,
            ConfigurationService configurationService,
            ScoreService scoreService,
            DataService dataService,
            LoginService loginService,
            GameDataService gameDataService)
        {
            this.userStore = userStore;
            this.configurationService = configurationService;
            this.scoreService = scoreService;
            this.dataService = dataService;
            this.loginService = loginService;
            _gameDataService = gameDataService;
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
                    UserExists = false,
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
                    TwitchName = user.UserName,
                    Position = position
                };

                response.Score.AddRange(scoreEntries.Select(s => new Score { Position = s.Position, LatestScore = s.Score, TwitchName = s.Name }));
            }

            response.AllowedMods.AddRange(await allowedModsTask);

            return response;
        }

        public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            VerifyId(request.Id);
            var (secret, token) = loginService.CreateLoginToken(request.Id);

            return Task.FromResult(new LoginResponse()
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
            var user = await GetCachedUserAsync(request.Id);

            // NYI
            return new StartResponse()
            {
                GameFileHash = "837e504433e8f5ffa9283271e4906063",
                GameFileUrl = "https://mod.rimionship.com/game/rimionship.rws",
                StartingPawnCount = 5,
                Settings = DefaultSettings
            };
        }

        private Settings DefaultSettings => new Settings()
        {
            Rising = new()
            {
                MaxFreeColonistCount = 5,
                RisingCooldown = 120_000,
                RisingInterval = 2400_000,
                RisingIntervalMinimum = 120_000,
                RisingReductionPerColonist = 240_000
            },
            Punishment = new()
            {
                FinalPauseInterval = 10,
                MaxThoughtFactor = 3f,
                MinThoughtFactor = 1f,
                StartPauseInterval = 120_000
            },
            Traits = new()
            {
                BadTraitSuppression = 0.15f,
                GoodTraitSuppression = 0.7f,
                ScaleFactor = 0.2f
            }
        };

        public override async Task<FutureEventsResponse> FutureEvents(FutureEventsRequest request, ServerCallContext context)
        {
            var user = await GetCachedUserAsync(request.Id);
            // NYI
            return new FutureEventsResponse();
        }

        public override async Task<StatsResponse> Stats(StatsRequest request, ServerCallContext context)
        {
            var ct = context.CancellationToken;
            var user = await GetCachedUserAsync(request.Id, ct);

            // PARTIALLY implemented - at least, we keep the scores in-memory
            await this.scoreService.AddOrUpdateScoreAsync(request.Id, user.UserName, user.AvatarUrl, request.Wealth, ct);

            await _gameDataService.ProcessStatsRequestAsync(request, ct);
            
            return new StatsResponse { Interval = 10 };
        }

        public override async Task<SyncResponse> Sync(SyncRequest request, ServerCallContext context)
        {
            // Authentication required?
            // var user = await GetCachedUserAsync(request.Id);

            if (request.WaitForChange)
                await Task.Delay(-1, context.CancellationToken);

            return new SyncResponse()
            {
                Message = "NYI",
                State = new State
                {
                    Game = State.Types.Game.Training,
                    PlannedStartHour = 0,
                    PlannedStartMinute = 0
                },
                Settings = DefaultSettings
            };
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
