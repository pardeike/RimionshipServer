using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
	}
}
