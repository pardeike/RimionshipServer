using Microsoft.EntityFrameworkCore;
using RimionshipServer.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace RimionshipServer.Models
{
	public enum StateKey
	{
		ServerMessage,
		GameState,
		GameStartHour,
		GameStartMinute,
		ScaleFactor,
		GoodTraitSuppression,
		BadTraitSuppression,
		Rising,
		Punishment,
		MaxFreeColonistCount,
		RisingInterval,
		RandomStartPauseMin,
		RandomStartPauseMax,
		StartPauseInterval,
		FinalPauseInterval
	}

	public class PlayState
	{
		[Key]
		public string Key { get; set; }
		public string Value { get; set; }

		public static string GetString(StateKey key)
		{
			using var context = new DataContext();
			return context.PlayStates.Find(key.ToString())?.Value;
		}

		public static void SetString(StateKey key, string value)
		{
			using var context = new DataContext();
			var state = context.PlayStates.Find(key.ToString());
			if (state == null)
			{
				state = new PlayState() { Key = key.ToString(), Value = value };
				_ = context.PlayStates.Add(state);
			}
			else
				state.Value = value;
			_ = context.SaveChanges();
		}

		public static int GetInt(StateKey key)
		{
			using var context = new DataContext();
			if (int.TryParse(context.PlayStates.Find(key.ToString())?.Value, out var result))
				return result;
			return 0;
		}

		public static void SetInt(StateKey key, int value) => SetString(key, value.ToString());



		public static float GetFloat(StateKey key)
		{
			using var context = new DataContext();
			if (float.TryParse(context.PlayStates.Find(key.ToString())?.Value, out var result))
				return result;
			return 0;
		}

		public static void SetFloat(StateKey key, float value) => SetString(key, value.ToString());

		public static TEnum GetEnum<TEnum>(StateKey key) where TEnum : struct, Enum
		{
			using var context = new DataContext();
			var enumStr = context.PlayStates.Find(key.ToString())?.Value ?? "";
			return Enum.Parse<TEnum>(enumStr);
		}

		public static void SetEnum<TEnum>(StateKey key, TEnum value) where TEnum : struct, Enum
		{
			SetString(key, value.ToString());
		}
	}
}
