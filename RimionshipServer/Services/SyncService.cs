using Api;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RimionshipServer.Services
{
	public class SyncService
	{
		private readonly ILogger<SyncService> _logger;
		private SyncResponse lastSyncState = new();
		private readonly Dictionary<string, Channel<bool>> clientChannels = new();

		private static bool dataPusherActive = false;

		public SyncService(ILogger<SyncService> logger)
		{
			_logger = logger;
			CreateDebugDataPusher();
		}

		// ONLY FOR TESTING, REMOVE LATER
		public void CreateDebugDataPusher()
		{
			if (dataPusherActive) return;
			dataPusherActive = true;

			_ = Task.Run(async () =>
			{
				var flag = 0;
				while (true)
				{
					flag++;
					var state = (flag % 5) switch
					{
						0 => new SyncResponse() { Message = $"Test message {flag}" },
						1 => new SyncResponse()
						{
							State = new State()
							{
								Game = State.Types.Game.Prepare,
								PlannedStartHour = flag % 24,
								PlannedStartMinute = flag % 60
							}
						},
						2 => new SyncResponse() { Message = "" },
						3 => new SyncResponse()
						{
							State = new State()
							{
								Game = State.Types.Game.Started,
								PlannedStartHour = 0,
								PlannedStartMinute = 0
							}
						},
						4 => new SyncResponse()
						{
							Settings = new Settings()
							{
								Traits = new Traits()
								{
									ScaleFactor = 0.2f,
									GoodTraitSuppression = 0.7f,
									BadTraitSuppression = 0.15f
								},
								Rising = new Rising()
								{
									MaxFreeColonistCount = 5,
									RisingInterval = 120000
								},
								Punishment = new Punishment()
								{
									RandomStartPauseMin = 140,
									RandomStartPauseMax = 600,
									StartPauseInterval = 30000,
									FinalPauseInterval = 5000
								}
							}
						},
						_ => throw new System.NotImplementedException()
					};
					AddResponse(state);
					await Task.Delay(10000);
				}
			});
		}

		public void AddResponse(SyncResponse response)
		{
			lastSyncState = response;
			foreach (var client in clientChannels)
				_ = client.Value.Writer.TryWrite(true);
		}

		public async Task<SyncResponse> StateForClient(string twitchId)
		{
			if (clientChannels.TryGetValue(twitchId, out var channel))
				_ = await channel.Reader.ReadAsync();
			else
			{
				channel = Channel.CreateBounded<bool>(1);
				clientChannels.Add(twitchId, channel);
			}
			return lastSyncState;
		}

		public void RemoveClient(string twitchId)
		{
			_ = clientChannels.Remove(twitchId);
		}
	}
}
