using Microsoft.AspNetCore.Identity;
using RimionshipServer.Data;

namespace RimionshipServer.Users
{
	public interface IUserStore : IUserStore<RimionUser>, IUserLoginStore<RimionUser>
	{
	}
}
