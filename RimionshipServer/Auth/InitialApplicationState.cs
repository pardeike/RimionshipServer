using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RimionshipServer
{
	public class InitialApplicationState
	{
		public string ModID { get; set; }
		public string AccessToken { get; set; }

		public async Task Associate(HttpContext httpContext, HttpRequest request, HttpResponse response)
		{
			var user = httpContext.User;
			if (user.Identity.IsAuthenticated == false)
				return;

			AccessToken = await httpContext.GetTokenAsync("access_token");
			if (AccessToken.IsNotEmpty())
				return;

			var participant = await Participant.ForPrincipal(user);
			if (participant == null)
				return;

			var tempModId = request.Cookies["ModID"] ?? request.Query["id"].ToString();
			if (tempModId.IsNotEmpty() == false)
			{
				participant.Mod = tempModId;

				using var context = new DataContext();
				_ = context.Update(participant);
				_ = await context.SaveChangesAsync();

				Debug.WriteLine($"User {participant.TwitchName} [{participant.TwitchId}] with accessToken {AccessToken} associated with mod {tempModId}");
				response.Cookies.Delete("ModID");
			}

			ModID = participant.Mod;
		}
	}
}
