#if DEBUG
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RimionshipServer.Data;

namespace RimionshipServer
{
    [ApiController]
    [Authorize]
    public class HackController : ControllerBase
    {
        private SignInManager<RimionUser> signInManager;
        private UserManager<RimionUser> userManager;

        public HackController(SignInManager<RimionUser> signInManager, UserManager<RimionUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("nuke/me/plz")]
        public async Task<IActionResult> NukeMe()
        {
            var user = await userManager.GetUserAsync(this.User);
            await this.userManager.DeleteAsync(user);
            await this.signInManager.SignOutAsync();
            return Ok("GOODBYE");
        }
    }
}
#endif