using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;

namespace RimionshipServer.Pages.Api
{
    public class Laufbanner : PageModel
    {
        private readonly RimionDbContext _db;
        public Laufbanner(RimionDbContext db)
        {
            _db = db;
        }
        
        public IEnumerable<(int, string, double)> Entries { get; set; } = null!;
        public async Task<IActionResult> OnGetAsync()
        {
            Entries = await Stats.GetTopXNotBannedUserFromDynamicCacheWithValue(nameof(HistoryStats.Wealth), int.MaxValue, _db);
            return Page();
        }
    }
}