using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;

namespace RimionshipServer.Pages
{
    public class DeleteAccountModel : PageModel
    {
        private readonly SignInManager<RimionUser> signInManager;
        private readonly UserManager<RimionUser> userManager;

        public DeleteAccountModel(SignInManager<RimionUser> signInManager, UserManager<RimionUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("Kein Benutzerkonto vorhanden.");

            await signInManager.SignOutAsync();
            return RedirectToPage("/DeleteAccount");
        }
    }
}
