using Microsoft.EntityFrameworkCore;
using RimionshipServer.Services;
using RimionshipServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RimionshipServer.Models
{
	[Index(nameof(ParticipantId))]
	[Index(nameof(Created))]
	public class Stat
	{
		public long Id { get; set; }

		public long ParticipantId { get; set; }

		public Participant Participant { get; set; }

		public DateTime Created { get; set; }

		public int Wealth { get; set; }

		public int MapCount { get; set; }

		public int Colonists { get; set; }

		public int ColonistsNeedTending { get; set; }

		public int MedicalConditions { get; set; }

		public int Enemies { get; set; }

		public int WildAnimals { get; set; }

		public int TamedAnimals { get; set; }

		public int Visitors { get; set; }

		public int Prisoners { get; set; }

		public int DownedColonists { get; set; }

		public int MentalColonists { get; set; }

		public int Rooms { get; set; }

		public int Caravans { get; set; }

		public int WeaponDps { get; set; }

		public int Electricity { get; set; }

		public int Medicine { get; set; }

		public int Food { get; set; }

		public int Fire { get; set; }

		public int Conditions { get; set; }

		public int Temperature { get; set; }

		public int NumRaidsEnemy { get; set; }

		public int NumThreatBigs { get; set; }

		public int ColonistsKilled { get; set; }

		public int GreatestPopulation { get; set; }

		public int InGameHours { get; set; }

		public float DamageTakenPawns { get; set; }

		public float DamageTakenThings { get; set; }

		public float DamageDealt { get; set; }

		public static async Task<List<Api.Score>> WealthList()
		{
			using var context = new DataContext();
			return await context.FromSqlQuery("""
					SELECT DISTINCT
						p.TwitchName,
						LAST_VALUE ( s.Wealth ) OVER (
							PARTITION BY p.Id
							ORDER BY p.Id
						) AS LatestScore 
					FROM Stats s
					JOIN Participants p ON (s.ParticipantId = p.Id)
					ORDER BY LatestScore DESC
				""",
				row => new Api.Score()
				{
					TwitchName = row[0] as string,
					LatestScore = (int)((long)row[1])
				}
			).ToListAsync();
		}
	}
}
