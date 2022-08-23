namespace RimionshipServer.Data
{
	public class AllowedMod
	{
		public long Id { get; set; }
		public string PackageId { get; set; } = null!;
		public ulong SteamId { get; set; }
	}
}
