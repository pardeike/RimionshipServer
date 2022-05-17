using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RimionshipServer.Models
{
	[Index(nameof(TwitchId), IsUnique = true)]
	[Index(nameof(Mod))]
	public class Participant
	{
		public long Id { get; set; }

		public string TwitchId { get; set; }

		public string TwitchName { get; set; }

		public string Mod { get; set; }

		public List<Stat> Stats { get; } = new();

		public static async Task<Participant> ForNewModID(string modId)
		{
			using var context = new DataContext();
			return await context.Participants.FirstOrDefaultAsync(p => p.TwitchId == null && p.Mod == modId);
		}

		public static async Task<Participant> ForTwitchId(string twitchId)
		{
			using var context = new DataContext();
			return await context.Participants.FirstOrDefaultAsync(p => p.TwitchId == twitchId);
		}
	}
}
