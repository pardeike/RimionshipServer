using System.Security.Cryptography;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
namespace RimionshipServer.Pages.Admin
{
    [BindProperties]
    public class GraphConfigurator : PageModel
    {
        private readonly RandomNumberGenerator _secureRandom = RandomNumberGenerator.Create();
        private readonly RimionDbContext       _dbContext;
        
        public GraphConfigurator(RimionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Done { get; set; }
        
        public           string                AccessCode { get; set; }

        public string               LastTime             { get; set; }
        public List<SelectListItem> LastTimeListItems    { get; set; }
        
        public  List<SelectListItem> StattSelectListItems { get; set; }
        public  string               Statt                { get; set; }
        public  List<string>         UserIds              { get; set; } = new ();
        public  List<SelectListItem> UserSelectListItems { get; set; }
        private RimionUser[]         _rimionUser          { get; set; }
        public  DateTime             Start                { get; set; }
        public  DateTime             End                  { get; set; }
        public  int                  IntervalSeconds      { get; set; }

        public List<GraphData> AllGraphs { get; set; }
        public async Task<IActionResult> OnGet()
        {
            StattSelectListItems = Stats.FieldNames.Select(x => new SelectListItem(x, x)).ToList();
            LastTimeListItems = new List<SelectListItem>{
                                                            new ("5", (5 * 60).ToString()),
                                                            new ("10", (10 * 60).ToString()),
                                                            new ("30", (30  * 60).ToString()),
                                                            new ("All", (24 * 60 * 60).ToString()),
                                                            new ("DEBUG 1 year", (365 * 24 * 60 * 60).ToString())
                                                        };
            var stuff = await _dbContext.Users.Select(x => new{x.Id, x.UserName}).ToListAsync();
            UserSelectListItems = stuff.Select(x => new SelectListItem(x.UserName, x.Id)).ToList();
            AllGraphs           = await _dbContext.GraphData.Include(x => x.UsersReference).ToListAsync();
            return Page();
        }

        public Task<IActionResult> OnPostCreateTop10WealthAsync()
        {
            return CreateTop10(nameof(Stats.Wealth));
        }

        public Task<IActionResult> OnPostCreateTop10Async()
        {
            return CreateTop10(Statt);
        }
        
        [NonAction]
        private async Task<IActionResult> CreateTop10(string stat)
        {
            if (AccessCode is null || string.IsNullOrWhiteSpace(AccessCode) || HttpUtility.UrlEncode(AccessCode) != AccessCode)
            {
                ModelState.AddModelError("AccessCode", "Name must be set and can not contain forbidden URL symbols (i.E. / ? = < > etc.)");
                return await OnGet();
            }
            
            var graphData = await _dbContext.GraphData
                                            .Include(x => x.UsersReference)
                                            .FirstOrDefaultAsync(x => x.Accesscode == AccessCode) 
                         ?? new GraphData{
                                             Statt = Statt
                                         };
            if (LastTime is null || string.IsNullOrWhiteSpace(LastTime))
            { 
                return Forbid();
            }
            
            var seconds = int.Parse(LastTime);
            graphData.Start = DateTime.Now - TimeSpan.FromSeconds(seconds);
            graphData.End   = DateTime.Now;
            
            var users = _dbContext.HistoryStats.FromSqlRaw($@"
SELECT DISTINCT * FROM (SELECT DISTINCT * FROM (SELECT DISTINCT * FROM HistoryStats ORDER BY Timestamp DESC) GROUP BY UserId) ORDER BY {stat} DESC LIMIT 10
").Select(x => x.UserId);

            graphData.UsersReference  = await _dbContext.Users.Where(x => users.Contains(x.Id)).ToListAsync();
            graphData.IntervalSeconds = 10;
            return await CreateAsync(graphData);
        }

        [NonAction]
        public async Task<IActionResult> CreateAsync(GraphData data)
        {
            data.Accesscode = AccessCode;
            if (data.Id == 0)
            {
                _dbContext.GraphData.Add(data);
                await _dbContext.SaveChangesAsync();
                Done = true;
            }
            else
            {
                _dbContext.GraphData.Update(data);
                await _dbContext.SaveChangesAsync();
                Done = true;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteGraphAsync(int id)
        {
            var graph = await _dbContext.GraphData.FindAsync(id);
            if (graph == null)
                throw new ArgumentNullException();
            _dbContext.GraphData.Remove(graph);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/GraphConfigurator");
        }
    }
}