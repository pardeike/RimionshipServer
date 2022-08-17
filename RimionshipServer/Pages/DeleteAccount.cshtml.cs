using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;
using RimionshipServer.Services;
using RimionshipServer.Users;

namespace RimionshipServer.Pages
{
    public class DeleteAccountModel : PageModel
    {
        private readonly SignInManager<RimionUser> signInManager;
        private readonly UserManager userManager;
        private readonly DataService dataService;

        public DeleteAccountModel(
            SignInManager<RimionUser> signInManager, 
            UserManager userManager,
            DataService dataService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.dataService = dataService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("Kein Benutzerkonto vorhanden.");

            var playerId = await userManager.GetPlayerIdAsync(user);
            await userManager.DeleteAsync(user);
            if (playerId != null)
                dataService.InvalidatePlayerCache(playerId);

            await signInManager.SignOutAsync();
            return RedirectToPage("/DeleteAccount");
        }
    }
}
