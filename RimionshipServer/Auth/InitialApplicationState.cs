using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RimionshipServer.Common;
using RimionshipServer.Models;
using RimionshipServer.Services;
using System.Threading.Tasks;

namespace RimionshipServer.Auth
{
	public class InitialApplicationState
	{
		public string ModID { get; set; }
		public string AccessToken { get; set; }

		public async Task Associate(SyncService syncService, ILogger<APIService> logger, HttpContext httpContext, HttpRequest request, HttpResponse response)
		{
			var user = httpContext.User;
			if (user.Identity.IsAuthenticated == false)
				return;

			AccessToken = await httpContext.GetTokenAsync("access_token");
			if (AccessToken.IsEmpty())
				return;

			var participant = await Participant.ForPrincipal(user);
			if (participant == null)
				return;

			var tempModId = request.Cookies["ModID"] ?? request.Query["id"].ToString();
			if (tempModId.IsNotEmpty() && participant.Mod != tempModId)
			{
				participant.Mod = tempModId;

				using var context = new DataContext();
				_ = context.Update(participant);
				_ = await context.SaveChangesAsync();

				logger.LogInformation("User {TwitchName} [{TwitchId}] associated with mod {tempModId}",
					participant.TwitchName,
					participant.TwitchId,
					tempModId
				);

				syncService.Update(participant.TwitchName);

				response.Cookies.Delete("ModID");
			}

			ModID = participant.Mod;
		}
	}
}
