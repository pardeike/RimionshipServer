using Microsoft.AspNetCore.Identity;

namespace RimionshipServer.Data
{
	public class RimionUser : IdentityUser
	{
		public string? AvatarUrl { get; set; }

		public virtual LatestStats? LatestStats { get; set; }

        public bool WasBanned    { get; set; } = false;
        public bool IsSuspicious { get; set; } = false;
    }
}
