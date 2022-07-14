using Microsoft.EntityFrameworkCore;
using RimionshipServer.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RimionshipServer.Models
{
	[Index(nameof(TwitchId), IsUnique = true)]
	[Index(nameof(Mod))]
	public class Participant
	{
		const string schema_nameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
		const string schema_name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

		public long Id { get; set; }

		public string TwitchId { get; set; }

		public string TwitchName { get; set; }

		public string Mod { get; set; }

		public List<Stat> Stats { get; } = new();

		public static async Task<Participant> ForModId(string id)
		{
			using var context = new DataContext();
			return await context.Participants.FirstOrDefaultAsync(p => p.Mod == id);
		}

		public static async Task<Participant> ForPrincipal(ClaimsPrincipal user)
		{
			var twitchId = user.FindFirst(schema_nameIdentifier)?.Value;
			var twitchName = user.FindFirst(schema_name)?.Value;
			if (twitchId == null || twitchName == null)
				return null;

			using var context = new DataContext();
			var participant = await context.Participants.FirstOrDefaultAsync(p => p.TwitchId == twitchId);
			if (participant == null)
			{
				participant = new Participant() { TwitchId = twitchId, TwitchName = twitchName };
				_ = await context.Participants.AddAsync(participant);
				_ = await context.SaveChangesAsync();
			}
			return participant;
		}
	}
}
