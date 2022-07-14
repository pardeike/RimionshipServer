using Api;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RimionshipServer.Models
{
	public class AllowedMod
	{
		public int Id { get; set; }
		public string PackageId { get; set; }
		public ulong SteamId { get; set; }

		public AllowedMod(int id, string packageId, ulong steamId)
		{
			Id = id;
			PackageId = packageId;
			SteamId = steamId;
		}

		public static async Task<List<Mod>> List()
		{
			using var context = new DataContext();
			return await context.AllowedMods
				.OrderBy(m => m.Id)
				.Select(m => new Mod() { PackageId = m.PackageId, SteamId = m.SteamId })
				.ToListAsync();
		}
	}
}
