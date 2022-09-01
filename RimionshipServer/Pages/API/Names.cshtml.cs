using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RimionshipServer.Pages.Api
{
    public class Names : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string[] UserNames { get; set; } = null!;
        
        [BindProperty(SupportsGet = true)]
        public bool[] BoolUserNames { get; set; } = null!;

        public void OnGet()
        {
            UserNames     =  new[]{"Brrainz", "Schemfil"};
            BoolUserNames = new bool[UserNames.Length];
        }
        
    }
}