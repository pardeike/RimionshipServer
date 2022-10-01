using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Pages.Admin;

namespace RimionshipServer.Pages.Api
{
    public class SimpleGraphHeading : PageModel
    {
        private readonly RimionDbContext _dbContext;
        public SimpleGraphHeading(RimionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty(SupportsGet = true)]
        public string GraphName { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public string GraphHeading { get; set; } = null!;
        
        public async Task<IActionResult> OnGet(string secret)
        {
            GraphHeading = await _dbContext.GraphData
                                           .AsNoTrackingWithIdentityResolution()
                                           .Include(x => x.UsersReference)
                                           .Where(x => x.Accesscode == secret)
                                           .Select(x => x.Statt)
                                           .FirstOrDefaultAsync();
            
            if (GraphHeading is null)
            {
                return NotFound();
            }
            GraphHeading = GraphConfigurator.StatsNames[GraphHeading];
            return Page();
        }
    }
}