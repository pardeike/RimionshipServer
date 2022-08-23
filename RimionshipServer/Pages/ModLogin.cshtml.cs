using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;
using RimionshipServer.Services;
using RimionshipServer.Users;

namespace RimionshipServer.Pages
{
	public class ModLoginModel : PageModel
	{
		private readonly SignInManager<RimionUser> signInManager;
		private readonly UserManager userManager;
		private readonly LoginService loginService;

		[TempData]
		public string? ErrorMessage { get; set; }

		public ModLoginModel(
			 SignInManager<RimionUser> signInManager,
			 UserManager userManager,
			 LoginService loginService)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.loginService = loginService;
		}

		public async Task<IActionResult> OnGet(string? token = null)
		{
			if (token == null)
				return Page();

			var user = signInManager.IsSignedIn(User) ? await userManager.GetUserAsync(User) : null;

			if (user == null)
			{
				var returnUrl = Url.Page("/ModLogin", values: new { token });
				var redirectUri = Url.Page("/ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
				var properties = signInManager.ConfigureExternalAuthenticationProperties("Twitch", redirectUri);
				return new ChallengeResult("Twitch", properties);
			}

			try
			{
				loginService.ActivateToken(token, user.Id);
				return Redirect("/ModLogin");
			}
			catch
			{
				ErrorMessage = "Es trat ein Fehler auf. Bitte versuche nochmals in dem Spiel dich einzuloggen.";
			}

			return Page();
		}
	}
}
