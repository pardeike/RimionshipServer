using System.Collections.Immutable;

namespace RimionshipServer.Services
{
	public record ScoreEntry(string ClientId, string Name, string? AvatarUrl, int Score);

	public record RelativeScoreEntry(int Position, string Name, int Score);

	public class ScoreService
	{
		private readonly SemaphoreSlim updateSemaphore = new(1);

		public ImmutableList<ScoreEntry> Scores { get; private set; } = ImmutableList.Create<ScoreEntry>();

		public async Task AddOrUpdateScoreAsync(string clientId, string name, string? avatarUrl, int score, CancellationToken cancellationToken = default)
		{
			var entry = new ScoreEntry(clientId, name, avatarUrl, score);
			await updateSemaphore.WaitAsync(cancellationToken);
			try
			{
				for (int i = 0; i < Scores.Count; ++i)
				{
					if (Scores[i].ClientId == clientId)
					{
						Scores = Scores.SetItem(i, entry);

						// Re-sort the array if necessary
						if (i > 0 && Scores[i - 1].Score < score || i < Scores.Count - 1 && Scores[i + 1].Score > score)
							Scores = Scores.Sort((a, b) => b.Score.CompareTo(a.Score));
						return;
					}
				}

				// Insert at the right location
				for (int i = 0; i < Scores.Count; ++i)
				{
					if (Scores[i].Score < score)
					{
						Scores = Scores.Insert(i, entry);
						return;
					}
				}

				Scores = Scores.Add(entry);
			}
			finally
			{
				updateSemaphore.Release();
			}
		}

		/// <summary>
		/// Returns the current position of a player as well as the scores of their neighbors
		/// </summary>
		public (int Position, IEnumerable<RelativeScoreEntry> RelativeScores) GetPlayerScoreData(string clientId)
		{
			var scores = Scores;
			var idx = GetPlayerIndex(scores, clientId);
			var scoreData = GetScoreEntriesForPlayer(scores, idx);

			return (idx + 1, scoreData);
		}
		private int GetPlayerIndex(ImmutableList<ScoreEntry> scores, string clientId)
		{
			int idx = scores.FindIndex(s => s.ClientId == clientId);
			return idx == -1 ? Scores.Count + 1 : idx;
		}

		private IEnumerable<RelativeScoreEntry> GetScoreEntriesForPlayer(ImmutableList<ScoreEntry> scores, int index)
		{
			var start = index - 1;
			if (start < 0)
				start = 0;

			var end = start + 2;
			if (end >= scores.Count)
				end = scores.Count - 1;

			while (start > 0 && end - start < 2)
				start--;

			for (var i = start; i <= end; ++i)
				yield return new RelativeScoreEntry(i + 1, scores[i].Name, scores[i].Score);
			for (var i = 0; i < 2 - end - start; i++)
				yield return new RelativeScoreEntry(0, "", 0);
		}
	}
}
