using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RimionshipServer.Common
{
	public static class Extensions
	{
		public static bool IsEmpty(this string str)
		{
			return str == null || str == "";
		}

		public static bool IsNotEmpty(this string str)
		{
			return str != null && str != "";
		}

		public static List<Api.Score> ShrinkedScoreList(this List<Api.Score> scores, string twitchName)
		{
			var place = scores.FindIndex(score => score.TwitchName == twitchName);
			if (place == -1)
			{
				scores.Add(new Api.Score() { TwitchName = twitchName, LatestScore = 0 });
				place = scores.Count - 1;
			}
			while (scores.Count < 3)
				scores.Add(new Api.Score() { TwitchName = "", LatestScore = 0 });
			var i1 = place - 1;
			var i2 = place + 1;
			while (i1 < 0)
			{
				i1++;
				i2++;
			}
			while (i2 >= scores.Count)
			{
				i1--;
				i2--;
			}
			return scores.GetRange(i1, 3);
		}
	}

	public static class Run
	{
		public static void Sync(Func<Task> function)
		{
			Task.Run(function).Wait();
		}
	}
}
