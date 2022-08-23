using Microsoft.AspNetCore.Identity;

namespace RimionshipServer.Users
{
	public class RoleManager : RoleManager<IdentityRole>
	{
		public RoleManager(
			 IRoleStore<IdentityRole> store,
			 IEnumerable<IRoleValidator<IdentityRole>> roleValidators,
			 ILookupNormalizer keyNormalizer,
			 IdentityErrorDescriber errors,
			 ILogger<RoleManager<IdentityRole>> logger)
			 : base(store, roleValidators, keyNormalizer, errors, logger)
		{
		}
	}
}
