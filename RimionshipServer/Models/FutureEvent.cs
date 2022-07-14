using Microsoft.EntityFrameworkCore;
using System;

namespace RimionshipServer.Models
{
	[Index(nameof(ParticipantId))]
	[Index(nameof(Ticks))]
	public class FutureEvent
	{
		public long Id { get; set; }

		public long ParticipantId { get; set; }

		public int Ticks { get; set; }

		public long ColonyWealth { get; set; }

		public string Name { get; set; }

		public string Faction { get; set; }

		public float Points { get; set; }

		public string Strategy { get; set; }

		public string ArrivalMode { get; set; }
	}
}
