using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;

namespace RimionshipServer.Pages.Api
{
    public class Carussel : PageModel
    {
        private readonly RimionDbContext _db;
        public Carussel(RimionDbContext db)
        {
            _db = db;
        }
        
        public IEnumerable<(int, string, double)> Entries { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int thing)
        {
            Entries = await Stats.GetTopXNotBannedUserFromDynamicCacheWithValue(thing switch {
                                                                                    1 => nameof(HistoryStats.TicksIgnoringBloodGod),
                                                                                    2 => nameof(HistoryStats.TicksLowColonistMood),
                                                                                    3 => nameof(HistoryStats.ColonistsKilled),
                                                                                    4 => nameof(HistoryStats.AmountBloodCleaned),
                                                                                    5 => nameof(HistoryStats.AnimalMeatCreated),
                                                                                    _ => throw new ArgumentOutOfRangeException(nameof(thing), thing, null)
                                                                                }, 10, _db);
            return Page();
        }
    }
}