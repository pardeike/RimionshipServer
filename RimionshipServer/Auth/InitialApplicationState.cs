using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RimionshipServer
{
	public class InitialApplicationState
	{
		const string schema_nameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
		const string schema_name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

		public string ModID { get; set; }
		public string UserID { get; set; }
		public string UserName { get; set; }
		public string AccessToken { get; set; }

		public bool CanAssociateModID() => ModID.IsNotEmpty() && AccessToken.IsNotEmpty();

		public void AssociateModID(HttpContext httpContext, HttpRequest request, HttpResponse response)
		{
			ModID = request.Cookies["ModID"] ?? request.Query["id"].ToString();
			UserID = httpContext.User.FindFirst(schema_nameIdentifier)?.Value;
			UserName = httpContext.User.FindFirst(schema_name)?.Value;

			var task1 = Task.Run(async () => AccessToken = await httpContext.GetTokenAsync("access_token"));
			task1.Wait();

			if (CanAssociateModID() && httpContext.User.Identity.IsAuthenticated)
			{
				Debug.WriteLine($"User {UserID} with accessToken {AccessToken} associated with mod {ModID}");
				response.Cookies.Delete("ModID");

				var task2 = Task.Run(async () => await Save(ModID, UserID, UserName));
				task2.Wait();
			}
		}

		public async Task Save(string modId, string userId, string userName)
		{
			using var context = new DataContext();
			var existingMod = await context.Participants.FirstOrDefaultAsync(p => p.Mod == modId);
			if (existingMod == null)
			{
				var existingParticipant = await context.Participants.FirstOrDefaultAsync(p => p.TwitchId == userId);
				if (existingParticipant == null)
					_ = await context.Participants.AddAsync(new Participant() { Mod = modId, TwitchId = userId, TwitchName = userName });
				else
				{
					existingParticipant.Mod = modId;
					existingParticipant.TwitchName = userName;
					_ = context.Participants.Update(existingParticipant);
				}
				_ = await context.SaveChangesAsync();
			}
		}
	}
}
