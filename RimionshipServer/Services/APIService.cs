using Api;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RimionshipServer.Models;
using RimionshipServer.Common;
using System;
using System.Threading.Tasks;

namespace RimionshipServer.Services
{
	//[Authorize]
	public class APIService : API.APIBase
	{
		private readonly SyncService _syncService;
		private readonly ILogger<APIService> _logger;

		public APIService(SyncService syncService, ILogger<APIService> logger)
		{
			_syncService = syncService;
			_logger = logger;
		}

		public async Task<Participant> GetParticipant(string id, string callName)
		{
			var participant = await Participant.ForModId(id);
			if (participant == null)
				throw new RpcException(new Status(StatusCode.PermissionDenied, "No such user"));
			_logger.LogInformation("{CallName} from {Twitchuser}", callName, participant.TwitchName);
			return participant;
		}

		public override async Task<HelloResponse> Hello(HelloRequest request, ServerCallContext context)
		{
			var participant = await Participant.ForModId(request.Id ?? "");
			var twitchName = participant?.TwitchName;
			var mods = await AllowedMod.List();
			var response = new HelloResponse() { Found = participant != null };
			response.AllowedMods.AddRange(mods);
			response.TwitchName = twitchName ?? "";
			if (twitchName!= null)
			{ 
				var scores = await Stat.WealthList();
				response.Score.AddRange(scores.ShrinkedScoreList(twitchName));
			}
			return response;
		}

		public override async Task<SyncResponse> Sync(SyncRequest request, ServerCallContext context)
		{
			var participant = await GetParticipant(request.Id, "sync");
			return await _syncService.StateForClient(participant.TwitchName);
		}

		public override async Task<StatsResponse> Stats(StatsRequest request, ServerCallContext context)
		{
			var participant = await GetParticipant(request.Id, "stats");
			using var dataContext = new DataContext();
			_ = await dataContext.Stats.AddAsync(new Stat()
			{
				ParticipantId = participant.Id,
				Created = DateTime.Now,
				Wealth = request.Wealth,
				MapCount = request.MapCount,
				Colonists = request.Colonists,
				ColonistsNeedTending = request.ColonistsNeedTending,
				MedicalConditions = request.MedicalConditions,
				Enemies = request.Enemies,
				WildAnimals = request.WildAnimals,
				TamedAnimals = request.TamedAnimals,
				Visitors = request.Visitors,
				Prisoners = request.Prisoners,
				DownedColonists = request.DownedColonists,
				MentalColonists = request.MentalColonists,
				Rooms = request.Rooms,
				Caravans = request.Caravans,
				WeaponDps = request.WeaponDps,
				Electricity = request.Electricity,
				Medicine = request.Medicine,
				Food = request.Food,
				Fire = request.Fire,
				Conditions = request.Conditions,
				Temperature = request.Temperature,
				NumRaidsEnemy = request.NumRaidsEnemy,
				NumThreatBigs = request.NumThreatBigs,
				ColonistsKilled = request.ColonistsKilled,
				GreatestPopulation = request.GreatestPopulation,
				InGameHours = request.InGameHours,
				DamageTakenPawns = request.DamageTakenPawns,
				DamageTakenThings = request.DamageTakenThings,
				DamageDealt = request.DamageDealt
			});
			_ = await dataContext.SaveChangesAsync();
			return new StatsResponse { Interval = 15 };
		}

		public override async Task<FutureEventsResponse> FutureEvents(FutureEventsRequest request, ServerCallContext context)
		{
			var participant = await GetParticipant(request.Id, "futureevents");
			using var dataContext = new DataContext();
			_ = await dataContext.Database.ExecuteSqlRawAsync(@$"DELETE FROM FutureEvents WHERE ParticipantId = {participant.Id}");
			var enumerator = request.Event.GetEnumerator();
			while(enumerator.MoveNext())
			{
				var futureEvent = enumerator.Current;
				_ = await dataContext.FutureEvents.AddAsync(new Models.FutureEvent()
				{
					ParticipantId = participant.Id,
					Ticks = futureEvent.Ticks,
					Name = futureEvent.Name,
					Faction = futureEvent.Faction,
					Points = futureEvent.Points,
					Strategy = futureEvent.Strategy,
					ArrivalMode = futureEvent.ArrivalMode,
				});
			}
			_ = await dataContext.SaveChangesAsync();
			return new FutureEventsResponse();
		}
	}
}
