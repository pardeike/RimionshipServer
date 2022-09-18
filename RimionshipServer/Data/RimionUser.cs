using Microsoft.AspNetCore.Identity;

namespace RimionshipServer.Data
{
	public class RimionUser : IdentityUser
	{
		public string? AvatarUrl { get; set; }

		public virtual LatestStats? LatestStats { get; set; }

        public bool WasBanned { get; set; }
        
        public bool HasQuit   { get; set; }

        public int WasShownTimes { get; set; }
        
        public virtual List<GraphData> InGraphs { get; set; } = new ();
    }
}