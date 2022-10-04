using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;

namespace RimionshipServer.Pages.Api
{
    public class ShowEnd : PageModel
    {
        private readonly RimionDbContext _dbContext;
        public ShowEnd(RimionDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> OnGet()
        {
            var wealth = await Stats.GetTopXNotBannedUserFromDynamicCacheWithValue(nameof(Stats.Wealth), Int32.MaxValue, _dbContext);
            var ret = wealth.OrderBy(x => x.Item3).Aggregate("Name;Koloniewert;\n", (y, x) =>  y + $"{x.Item2};{x.Item3};\n");
            return new OkObjectResult(ret);
        }
        
        public async Task<IActionResult> OnGetFull()
        {
            var wealth = await Stats.GetFinalResults(_dbContext);
            var ret    = "Name;" + Stats.FieldNames.Aggregate("", (s, fn) => s + fn + ";") + "\n";

            ret = (wealth.OrderByDescending(x => x.Key)
                         .Select(pair => new{pair, result = pair.Key + ";"})
                         .Select(t => Stats.FieldNames.Aggregate(t.result, (current, value) => current + t.pair.Value[value] + ";")))
               .Aggregate(ret, (current1, result) => current1 + result + "\n");

            return new OkObjectResult(ret);
        }
    }
}