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
		const string schema_nameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
		const string schema_name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

		public string ModID { get; set; }
		public string AccessToken { get; set; }

		public async Task AssociateModID(HttpContext httpContext, HttpRequest request, HttpResponse response)
		{
			var user = httpContext.User;
			if (user.Identity.IsAuthenticated == false)
				return;

			var tempModId = request.Cookies["ModID"] ?? request.Query["id"].ToString();
			AccessToken = await httpContext.GetTokenAsync("access_token");

			var twitchId = user.FindFirst(schema_nameIdentifier)?.Value;
			var twitchName = user.FindFirst(schema_name)?.Value;

			using var context = new DataContext();
			if (tempModId.IsNotEmpty() && AccessToken.IsNotEmpty())
			{
				var participant = await Participant.ForNewModID(tempModId);
				if (participant != null)
				{
					await context.Participants.Where(p => p.TwitchId == twitchId).ForEachAsync(p => context.Remove(p));
					participant.TwitchId = twitchId;
					participant.TwitchName = twitchName;
					_ = context.Update(participant);
					_ = await context.SaveChangesAsync();

					Debug.WriteLine($"User {twitchName} / {twitchId} with accessToken {AccessToken} associated with mod {ModID}");
					response.Cookies.Delete("ModID");
				}
			}

			var current = await Participant.ForTwitchId(twitchId);
			if (current != null)
				ModID = current.Mod;
		}
	}
}
