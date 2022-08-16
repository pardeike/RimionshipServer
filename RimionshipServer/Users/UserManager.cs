using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RimionshipServer.Data;

namespace RimionshipServer.Users
{
    public class UserManager : UserManager<RimionUser>
    {
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
    }
}
