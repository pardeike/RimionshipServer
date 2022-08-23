namespace RimionshipServer.Data
{
	public abstract partial class Stats
	{
		public string UserId { get; set; } = null!;

		public DateTimeOffset Timestamp { get; set; }

		public virtual RimionUser User { get; set; } = null!;
	}
}
