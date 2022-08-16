using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;

namespace RimionshipServer.Users
{
    public class UserStore : UserStore<RimionUser>, IUserStore
    {
        public UserStore(
            RimionDbContext context, 
            IdentityErrorDescriber describer = null!)
            : base(context, describer)
        {
        }
    }
}
