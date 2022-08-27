using Grpc.Core;
using Microsoft.Extensions.Options;
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
		private readonly DataService dataService;
		private readonly LoginService loginService;
		private readonly IOptions<RimionshipOptions> options;
		private readonly AttentionService _attention;

		public GrpcService(
			 RimionDbContext db,
			 ConfigurationService configurationService,
			 ScoreService scoreService,
			 DataService dataService,
			 LoginService loginService,
			 IOptions<RimionshipOptions> options,
			 AttentionService attention)
		{
			this.db = db;
			this.configurationService = configurationService;
			this.scoreService = scoreService;
			this.dataService = dataService;
			this.loginService = loginService;
			this.options = options;
			_attention = attention;
		}
		/**
		 * Increases the Attention Score for player X by Y amount
		 */
		public override Task<AttentionResponse> Attention(AttentionRequest request, ServerCallContext context)
		{
			VerifyId(request.Id);
			_attention.IncreaseAttentionScore(request.Id, request.Delta);
			return Task.FromResult(new AttentionResponse());
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
				GameFileHash = options.Value.GameFileHash,
				GameFileUrl = options.Value.GameFileUrl,
				StartingPawnCount = 5,
				Settings = DefaultSettings
			};
		}

		private Settings DefaultSettings => new()
		{
			Rising = new()
			{
				MaxFreeColonistCount = 5,
				RisingCooldown = 0,
				RisingInterval = 1200_000,
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
				ScaleFactor = 0.2f,
				MaxMeleeSkill = 6,
				MaxMeleeFlames = 1,
				MaxShootingFlames = 1,
				MaxShootingSkill = 6
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
			var user = await GetCachedUserAsync(request.Id);

			// PARTIALLY implemented - at least, we keep the scores in-memory
			await this.scoreService.AddOrUpdateScoreAsync(request.Id, user.UserName, user.AvatarUrl, request.Wealth, ct);

			await db.AddOrUpdateStatsAsync(user, request);
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
				Message = options.Value.SyncMessage,
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

		private async Task<RimionUser> GetCachedUserAsync(string clientId)
		{
			VerifyId(clientId);

			return await dataService.GetCachedUserByPlayerIdAsync(clientId) ?? throw new RpcException(new Status(StatusCode.Unauthenticated, "User not found"));
		}
	}
}
