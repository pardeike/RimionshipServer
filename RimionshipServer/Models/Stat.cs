using Microsoft.EntityFrameworkCore;
using System;

namespace RimionshipServer.Models
{
	[Index(nameof(Created))]
	public class Stat
	{
		public long Id { get; set; }

		public DateTime Created { get; set; }
		public long ColonyWealth { get; set; }
	}
}
