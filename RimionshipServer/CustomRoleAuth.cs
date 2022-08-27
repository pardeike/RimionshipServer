using Microsoft.AspNetCore.Authorization;
using RimionshipServer.Users;
namespace RimionshipServer
{
    public class CustomRoleAuth : IAuthorizationRequirement
    {
        public CustomRoleAuth(string role)
        {
            Role = role;
        }
        public string Role { get; set; }
    }
    public class CustomRoleAuthHandler : AuthorizationHandler<CustomRoleAuth>
    {
        private readonly UserManager _manager;
        
        public CustomRoleAuthHandler(UserManager manager)
        {
            _manager = manager;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRoleAuth requirement)
        {
            if (context.User is null)
            {
                context.Fail();
                return;
            }
            var user = await _manager.GetUserAsync(context.User);
            if (user is null)
            {
                context.Fail();
                return;
            }
            var roles = await _manager.GetRolesAsync(user);
            if (roles is null || roles.Count == 0)
            {
                context.Fail();
                return;
            }
            if (roles.Contains(requirement.Role))
            {
                context.Succeed(requirement);
                return;
            }
            context.Fail();
        }
    }
}