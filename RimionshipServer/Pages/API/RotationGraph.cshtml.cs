using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;

namespace RimionshipServer.Pages.Api
{
    [BindProperties(SupportsGet = true)]
    public class RotationGraph : PageModel
    {
        private readonly RimionDbContext _dbContext;
        
        public string GraphAccessCode { get; set; } = null!;
        public int    Maximum         { get; set; }
        public int    TimeToDisplay         { get; set; }
        
        public RotationGraph(RimionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync(string name, int id)
        {
            var rotation = await _dbContext.GraphRotationData
                                           .Include(x => x.ToRotate)
                                           .AsNoTrackingWithIdentityResolution()
                                           .Where(x => x.RotationName == name)
                                           .FirstAsync();
            
            TimeToDisplay = rotation.TimeToDisplay;
            Maximum       = rotation.ToRotate.Count();
            GraphAccessCode = rotation.ToRotate
                                      .Select(x => x.Accesscode)
                                      .Skip(id)
                                      .First();
            return Page();
        }
    }
}