using Microsoft.AspNetCore.Authorization;
using RimionshipServer.Users;
namespace RimionshipServer
{
    public class CustomRoleAuth : IAuthorizationRequirement {}
    public class CustomRoleAuthHandler : AuthorizationHandler<CustomRoleAuth>
    {
        private readonly UserManager _manager;
        
        public CustomRoleAuthHandler(UserManager manager)
        {
            _manager = manager;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRoleAuth requirement)
        {
            if ((await _manager.GetRolesAsync(await _manager.GetUserAsync(context.User))).Contains(Roles.Admin))
            {
                context.Succeed(requirement);
                return;
            }
            context.Fail();
        }
    }
}