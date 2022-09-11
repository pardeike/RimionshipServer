using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;
namespace RimionshipServer.Pages.Api
{
    public class ShowStream : PageModel
    {
        private RimionDbContext _db;
        public ShowStream(RimionDbContext db)
        {
            _db = db;
        }
        
        public async Task<IActionResult> OnGetStreamer()
        {
            var streamer = await _db.GetStreamerAsync(HttpContext.RequestAborted);
            return new JsonResult(new {streamer});
        }
    }
}