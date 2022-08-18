using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Users;
using System.Security.Claims;

namespace RimionshipServer
{
    public static class Extensions
    {
        public static string? GetTwitchName(this ClaimsPrincipal principal)
            => principal.FindFirstValue("urn:twitch:displayname");
    }
}
