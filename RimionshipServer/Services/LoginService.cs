using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using RimionshipServer.Data;
using RimionshipServer.Users;
using System.Security.Cryptography;
using System.Text.Json;

namespace RimionshipServer.Services
{
    public class LoginService
    {
        private readonly IUserStore userStore;
        private readonly IDataProtector dataProtector;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<LoginService> logger;

        private record LoginToken(string Secret, string PlayerId, DateTimeOffset IssueTime);
        private record ActivatedToken(string PlayerId, string UserId);

        public LoginService(
            IUserStore userStore,
            IDataProtectionProvider dataProtectionProvider,
            IMemoryCache memoryCache,
            ILogger<LoginService> logger)
        {
            this.userStore = userStore;
            this.dataProtector = dataProtectionProvider.CreateProtector(GetType().FullName!);
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        /// <summary>
        /// Issues a login token that can be used to link a Twitch account.
        /// </summary>
        public (string Secret, string Token) CreateLoginToken(string playerId)
        {
            // Create a token that we'll reference this entity for
            string secret;
            using (var rng = RandomNumberGenerator.Create())
            {
                Span<byte> tokenData = stackalloc byte[16];
                rng.GetBytes(tokenData);
                secret = Convert.ToBase64String(tokenData);
            }

            var token = new LoginToken(secret, playerId, DateTimeOffset.UtcNow);
            var protectedToken = dataProtector.Protect(JsonSerializer.Serialize(token));
            return (secret, protectedToken);
        }

        /// <summary>
        /// Activates a login token, by linking it to a valid user.
        /// </summary>
        public void ActivateToken(string token, string userId)
        {
            var loginToken = JsonSerializer.Deserialize<LoginToken>(dataProtector.Unprotect(token));

            if (DateTimeOffset.UtcNow.Subtract(loginToken!.IssueTime).TotalMinutes > 5)
            {
                logger.LogInformation("Attempted to activate expired token for {UserId} (issued at {IssueTime})", userId, loginToken.IssueTime);
                throw new ArgumentException(nameof(token));
            }

            var activatedToken = new ActivatedToken(loginToken.PlayerId, userId);
            using var entry = memoryCache.CreateEntry(GetCacheKey(loginToken.Secret));
            entry.SetPriority(CacheItemPriority.NeverRemove);
            entry.SetValue(activatedToken);
            entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
            //memoryCache.Set(GetCacheKey(loginToken.Secret), activatedToken, TimeSpan.FromSeconds(60));
        }

        public async Task<string?> RedeemTokenAsync(string playerId, string secret, CancellationToken cancellationToken = default)
        {
            if (!memoryCache.TryGetValue<ActivatedToken>(GetCacheKey(secret), out var activatedToken))
                return null;

            if (playerId != activatedToken.PlayerId)
            {
                logger.LogInformation("Player {PlayerId} tried to activate a token for {UserId}, which does not belong to him", playerId, activatedToken.UserId);
                return null;
            }

            var user = await userStore.FindByIdAsync(activatedToken.UserId, cancellationToken);
            if (user == null)
            {
                logger.LogInformation("Could not activate token: User {UserId} does no longer exist in the database", activatedToken.UserId);
                return null;
            }

            await userStore.AddClientIdLoginAsync(user, playerId, cancellationToken);
            memoryCache.Remove(GetCacheKey(secret));

            return user.UserName;
        }

        private string GetCacheKey(string secret)
            => $"RimionshipServer.LoginTokens.{secret}";
    }
}
