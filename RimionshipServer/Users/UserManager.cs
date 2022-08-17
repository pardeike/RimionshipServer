using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RimionshipServer.Data;

namespace RimionshipServer.Users
{
    public class UserManager : UserManager<RimionUser>
    {
        private const string ModLoginProvider = "RimionshipMod";

        public UserManager(
            IUserStore store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<RimionUser> passwordHasher, 
            IEnumerable<IUserValidator<RimionUser>> userValidators, 
            IEnumerable<IPasswordValidator<RimionUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<RimionUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public Task<RimionUser> FindByPlayerIdAsync(string playerId)
            => this.FindByLoginAsync(ModLoginProvider, playerId);

        public Task<IdentityResult> AddPlayerIdAsync(RimionUser user, string playerId)
            => this.AddLoginAsync(user, new UserLoginInfo(ModLoginProvider, playerId, null));
    }
}
