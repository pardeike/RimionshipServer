using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;
using System.Net;
namespace RimionshipServer.Pages.Api
{
    public class SaveFilePageModel : PageModel
    {
        public static SaveFile? SafeFile { get; set; }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public IActionResult OnGetFile()
        {
            if (SafeFile is null)
                return new NoContentResult();
            return File(SafeFile.File, "application/gzip", WebUtility.UrlDecode(SafeFile.Name) + ".gz");
        }

        public IActionResult OnGetHash()
        {
            if (SafeFile is null)
                return new NoContentResult();
            return new ObjectResult(SafeFile.MD5);
        }
    }
}