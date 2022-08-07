using Api;
using Microsoft.Extensions.Logging;
using RimionshipServer.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RimionshipServer.Services
{
	public class SyncService
	{
		private readonly ILogger<APIService> _logger;

		private readonly SyncResponse lastSyncState;
		private readonly Dictionary<string, Channel<bool>> clientChannels = new();

		public SyncService(ILogger<APIService> logger)
		{
			_logger = logger;

			var (gameState, hour, minute) = GameState;
			var (scaleFactor, goodTraitSuppression, badTraitSuppression) = Traits;
			var (maxFreeColonistCount, risingInterval, risingCooldown) = Rising;
			var (startPauseInterval, finalPauseInterval, minThoughtFactor, maxThoughtFactor) = Punishment;
			lastSyncState = new()
			{
				Message = ServerMessage,
				State = new()
				{
					Game = gameState,
					PlannedStartHour = hour,
					PlannedStartMinute = minute
				},
				Settings = new()
				{
					Traits = new()
					{
						ScaleFactor = scaleFactor,
						GoodTraitSuppression = goodTraitSuppression,
						BadTraitSuppression = badTraitSuppression
					},
					Rising = new()
					{
						MaxFreeColonistCount = maxFreeColonistCount,
						RisingInterval = risingInterval,
						RisingCooldown = risingCooldown
					},
					Punishment = new()
					{
						MinThoughtFactor = minThoughtFactor,
						MaxThoughtFactor = maxThoughtFactor
					}
				}
			};
		}

		public SyncResponse Latest()
		{
			return lastSyncState;
		}

		public Settings LatestSettings()
		{
			return lastSyncState.Settings;
		}

		public void Update(string twitchName)
		{
			if (clientChannels.TryGetValue(twitchName, out var channel))
				_ = clientChannels.Remove(twitchName);
		}

		public void Update(Action<SyncResponse> modifier = null)
		{
			modifier?.Invoke(lastSyncState);
			foreach (var client in clientChannels)
				_ = client.Value.Writer.TryWrite(true);
		}

		public async Task<SyncResponse> WaitForSyncResponseChange(string twitchName, CancellationToken cancellationToken)
		{
			if (clientChannels.TryGetValue(twitchName, out var channel))
			{
				try
				{
					await foreach (var _ in channel.Reader.ReadAllAsync(cancellationToken))
						;
				}
				catch (OperationCanceledException)
				{
				}
			}
			else
			{
				channel = Channel.CreateBounded<bool>(1);
				clientChannels.Add(twitchName, channel);
			}
			return lastSyncState;
		}

		public void RemoveClient(string twitchName)
		{
			_ = clientChannels.Remove(twitchName);
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

		public (int maxFreeColonistCount, int risingInterval, int risingCooldown) Rising
		{
			get =>
			(
				PlayState.GetInt(StateKey.MaxFreeColonistCount),
				PlayState.GetInt(StateKey.RisingInterval),
				PlayState.GetInt(StateKey.RisingCooldown)
			);
			set
			{
				PlayState.SetInt(StateKey.MaxFreeColonistCount, value.maxFreeColonistCount);
				PlayState.SetInt(StateKey.RisingInterval, value.risingInterval);
				PlayState.SetInt(StateKey.RisingCooldown, value.risingCooldown);
				Update(state =>
				{
					state.Settings ??= new Settings();
					state.Settings.Rising ??= new Rising();
					state.Settings.Rising.MaxFreeColonistCount = value.maxFreeColonistCount;
					state.Settings.Rising.RisingInterval = value.risingInterval;
					state.Settings.Rising.RisingCooldown = value.risingCooldown;
				});
			}
		}

		public (int startPauseInterval, int finalPauseInterval, float minThoughtFactor, float maxThoughtFactor) Punishment
		{
			get =>
			(
				PlayState.GetInt(StateKey.StartPauseInterval),
				PlayState.GetInt(StateKey.FinalPauseInterval),
				PlayState.GetInt(StateKey.MinThoughtFactor),
				PlayState.GetInt(StateKey.MaxThoughtFactor)
			);
			set
			{
				PlayState.SetInt(StateKey.StartPauseInterval, value.startPauseInterval);
				PlayState.SetInt(StateKey.FinalPauseInterval, value.finalPauseInterval);
				PlayState.SetFloat(StateKey.MinThoughtFactor, value.minThoughtFactor);
				PlayState.SetFloat(StateKey.MaxThoughtFactor, value.maxThoughtFactor);
				Update(state =>
				{
					state.Settings ??= new Settings();
					state.Settings.Punishment ??= new Punishment();
					state.Settings.Punishment.StartPauseInterval = value.startPauseInterval;
					state.Settings.Punishment.FinalPauseInterval = value.finalPauseInterval;
					state.Settings.Punishment.MinThoughtFactor = value.minThoughtFactor;
					state.Settings.Punishment.MaxThoughtFactor = value.maxThoughtFactor;
				});
			}
		}
	}
}
