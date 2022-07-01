using Microsoft.EntityFrameworkCore;
using System;

namespace RimionshipServer.Models
{
	[Index(nameof(ParticipantId))]
	[Index(nameof(Created))]
	public class Stat
	{
		public long Id { get; set; }

		public long ParticipantId { get; set; }

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

	}
}
