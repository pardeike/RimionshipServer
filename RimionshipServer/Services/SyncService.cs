using Api;
using RimionshipServer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RimionshipServer.Services
{
	public class SyncService
	{
		private readonly SyncResponse lastSyncState = new();
		private readonly Dictionary<string, Channel<bool>> clientChannels = new();

		private static bool dataPusherActive;

		public SyncService()
		{
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
					switch (flag % 5)
					{
						case 0:
							ServerMessage = $"Test message {flag}";
							break;
						case 1:
							GameState = (State.Types.Game.Prepare, flag % 24, flag % 60);
							break;
						case 2:
							ServerMessage = "";
							break;
						case 3:
							GameState = (State.Types.Game.Started, 0, 0);
							break;
						case 4:
							Traits = (0.2f, 0.7f, 0.15f);
							Rising = (5, 120000);
							Punishment = (140, 600, 30000, 5000);
							break;
					};
					await Task.Delay(2000);
				}
			});
		}

		public void Update(Action<SyncResponse> modifier)
		{
			modifier(lastSyncState);
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

		public string ServerMessage
		{
			get => PlayState.GetString(StateKey.ServerMessage);
			set
			{
				PlayState.SetString(StateKey.ServerMessage, value);
				Update(state => state.Message = value);
			}
		}

		public (State.Types.Game gameState, int hour, int minute) GameState
		{
			get =>
			(
				PlayState.GetEnum<State.Types.Game>(StateKey.GameState),
				PlayState.GetInt(StateKey.GameStartHour),
				PlayState.GetInt(StateKey.GameStartMinute)
			);
			set
			{
				PlayState.SetEnum(StateKey.GameState, value.gameState);
				PlayState.SetInt(StateKey.GameStartHour, value.hour);
				PlayState.SetInt(StateKey.GameStartMinute, value.minute);
				Update(state =>
				{
					state.State ??= new State();
					state.State.Game = value.gameState;
					state.State.PlannedStartHour = value.hour;
					state.State.PlannedStartMinute = value.minute;
				});
			}
		}

		public (float scaleFactor, float goodTraitSuppression, float badTraitSuppression) Traits
		{
			get =>
			(
				PlayState.GetFloat(StateKey.ScaleFactor),
				PlayState.GetFloat(StateKey.GoodTraitSuppression),
				PlayState.GetFloat(StateKey.BadTraitSuppression)
			);
			set
			{
				PlayState.SetFloat(StateKey.ScaleFactor, value.scaleFactor);
				PlayState.SetFloat(StateKey.GoodTraitSuppression, value.goodTraitSuppression);
				PlayState.SetFloat(StateKey.BadTraitSuppression, value.badTraitSuppression);
				Update(state =>
				{
					state.Settings ??= new Settings();
					state.Settings.Traits ??= new Traits();
					state.Settings.Traits.ScaleFactor = value.scaleFactor;
					state.Settings.Traits.ScaleFactor = value.goodTraitSuppression;
					state.Settings.Traits.ScaleFactor = value.badTraitSuppression;
				});
			}
		}

		public (int maxFreeColonistCount, int risingInterval) Rising
		{
			get =>
			(
				PlayState.GetInt(StateKey.MaxFreeColonistCount),
				PlayState.GetInt(StateKey.RisingInterval)
			);
			set
			{
				PlayState.SetInt(StateKey.MaxFreeColonistCount, value.maxFreeColonistCount);
				PlayState.SetInt(StateKey.RisingInterval, value.risingInterval);
				Update(state =>
				{
					state.Settings ??= new Settings();
					state.Settings.Rising ??= new Rising();
					state.Settings.Rising.MaxFreeColonistCount = value.maxFreeColonistCount;
					state.Settings.Rising.RisingInterval = value.risingInterval;
				});
			}
		}

		public (int randomStartPauseMin, int randomStartPauseMax, int startPauseInterval, int finalPauseInterval) Punishment
		{
			get =>
			(
				PlayState.GetInt(StateKey.RandomStartPauseMin),
				PlayState.GetInt(StateKey.RandomStartPauseMax),
				PlayState.GetInt(StateKey.StartPauseInterval),
				PlayState.GetInt(StateKey.FinalPauseInterval)
			);
			set
			{
				PlayState.SetInt(StateKey.RandomStartPauseMin, value.randomStartPauseMin);
				PlayState.SetInt(StateKey.RandomStartPauseMax, value.randomStartPauseMax);
				PlayState.SetInt(StateKey.StartPauseInterval, value.startPauseInterval);
				PlayState.SetInt(StateKey.FinalPauseInterval, value.finalPauseInterval);
				Update(state =>
				{
					state.Settings ??= new Settings();
					state.Settings.Punishment ??= new Punishment();
					state.Settings.Punishment.RandomStartPauseMin = value.randomStartPauseMin;
					state.Settings.Punishment.RandomStartPauseMax = value.randomStartPauseMax;
					state.Settings.Punishment.StartPauseInterval = value.startPauseInterval;
					state.Settings.Punishment.FinalPauseInterval = value.finalPauseInterval;
				});
			}
		}
	}
}
