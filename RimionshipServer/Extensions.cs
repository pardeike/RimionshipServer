using System.Security.Claims;

namespace RimionshipServer
{
	public static class Extensions
	{
		public static string? GetTwitchName(this ClaimsPrincipal principal)
			 => principal.FindFirstValue("urn:twitch:displayname");
	}
}
