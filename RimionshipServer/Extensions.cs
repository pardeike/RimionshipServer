using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Users;
using System.Security.Claims;

namespace RimionshipServer
{
    public static class Extensions
    {
        private const string ModLoginProvider = "RimionshipMod";

        public static string? GetTwitchName(this ClaimsPrincipal principal)
            => principal.FindFirstValue("urn:twitch:displayname");

        public static async Task<RimionUser> FindUserByClientIdAsync(
            this IUserStore store, 
            string modId,
            CancellationToken cancellationToken = default)
            => await store.FindByLoginAsync(ModLoginProvider, modId, cancellationToken);

        public static async Task AddClientIdLoginAsync(
            this IUserStore userStore, 
            RimionUser user, 
            string modId, 
            CancellationToken cancellationToken)
            => await userStore.AddLoginAsync(user, new UserLoginInfo(ModLoginProvider, modId, null), cancellationToken);
    }
}
