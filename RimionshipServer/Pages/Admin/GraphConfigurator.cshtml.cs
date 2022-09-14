using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
using System.Security.Cryptography;
using System.Web;
namespace RimionshipServer.Pages.Admin
{
    [BindProperties]
    public class GraphConfigurator : PageModel
    {
        private readonly RandomNumberGenerator _secureRandom = RandomNumberGenerator.Create();
        private readonly RimionDbContext _dbContext;

        private static readonly Dictionary<string, string> statsNames = new()
        {
            { nameof(Stats.Wealth), "Koloniewert" },
            { nameof(Stats.MapCount), "Maps" },
            { nameof(Stats.Colonists), "Kolonisten" },
            { nameof(Stats.ColonistsNeedTending), "Verwundete Kolonisten" },
            { nameof(Stats.MedicalConditions), "Erkrankte Kolonisten" },
            { nameof(Stats.Enemies), "Feinde" },
            { nameof(Stats.WildAnimals), "Wilde Tiere" },
            { nameof(Stats.TamedAnimals), "Zahme Tiere" },
            { nameof(Stats.Visitors), "Besucher" },
            { nameof(Stats.Prisoners), "Gefangene" },
            { nameof(Stats.DownedColonists), "Bewustlose Kolonisten" },
            { nameof(Stats.MentalColonists), "Ausgeflippte Kolonisten" },
            { nameof(Stats.Rooms), "Bebauter Fläche" },
            { nameof(Stats.Caravans), "Kolonisten in Karavanen" },
            { nameof(Stats.WeaponDps), "Waffenwert" },
            { nameof(Stats.Electricity), "Verfügbarer Strom" },
            { nameof(Stats.Medicine), "Medizin" },
            { nameof(Stats.Food), "Essen" },
            { nameof(Stats.Fire), "Feuer" },
            { nameof(Stats.Conditions), "Umweltbedingungen" },
            { nameof(Stats.Temperature), "Temperatur" },
            { nameof(Stats.NumRaidsEnemy), "Überfälle" },
            { nameof(Stats.NumThreatBigs), "Große Gefahren" },
            { nameof(Stats.ColonistsKilled), "Verluste" },
            { nameof(Stats.GreatestPopulation), "Größte Population" },
            { nameof(Stats.InGameHours), "Gespielte Stunden" },
            { nameof(Stats.DamageTakenPawns), "Personenschaden" },
            { nameof(Stats.DamageTakenThings), "Sachschaden" },
            { nameof(Stats.DamageDealt), "Beschädigung ausgeteilt" },
            { nameof(Stats.AnimalMeatCreated), "Fleisch erzeugt" },
            { nameof(Stats.AmountBloodCleaned), "Blut gereinigt" },
            { nameof(Stats.TicksLowColonistMood), "Zeit in mieser Stimmung" },
            { nameof(Stats.TicksIgnoringBloodGod), "Zeit ignorierter Blutgott" },
        };

        public GraphConfigurator(RimionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Done { get; set; }

        public string AccessCode { get; set; }

        public string LastTime { get; set; }
        public List<SelectListItem> LastTimeListItems { get; set; }

        public List<SelectListItem> StattSelectListItems { get; set; }
        public string Statt { get; set; }
        public List<string> UserIds { get; set; } = new();
        public List<SelectListItem> UserSelectListItems { get; set; }
        private RimionUser[] _rimionUser { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int IntervalSeconds { get; set; }
        public int CountUser { get; set; }
        public bool Autorefresh { get; set; }
        public List<GraphData> AllGraphs { get; set; }

        public static string StatsName(string key) => statsNames.ContainsKey(key) ? statsNames[key] : key;
        public static string NowNoSeconds() => DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        public async Task<IActionResult> OnGet()
        {
            StattSelectListItems = Stats.FieldNames.Select(x => new SelectListItem(StatsName(x), x)).ToList();
            LastTimeListItems = new List<SelectListItem>{
                                                            new ("5", (5              * 60).ToString()),
                                                            new ("10", (10            * 60).ToString()),
                                                            new ("30", (30            * 60).ToString()),
                                                            new ("60", (60            * 60).ToString()),
                                                            new ("120", (120          * 60).ToString()),
                                                            new ("240", (240            * 60).ToString()),
                                                            new ("All", (24           * 60 * 60).ToString()),
                                                            new ("DEBUG 1 year", (365 * 24 * 60 * 60).ToString())
                                                        };
            var stuff = await _dbContext.Users.Select(x => new { x.Id, x.UserName }).ToListAsync();
            UserSelectListItems = stuff.Select(x => new SelectListItem(x.UserName, x.Id)).ToList();
            AllGraphs = await _dbContext.GraphData.Include(x => x.UsersReference).ToListAsync();
            CountUser = 10;
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
                         ?? new GraphData();

            if (LastTime is null || string.IsNullOrWhiteSpace(LastTime))
            {
                return Forbid();
            }

            return await CreateAsync(graphData);
        }

        [NonAction]
        public async Task<IActionResult> CreateAsync(GraphData data)
        {
            var seconds = int.Parse(LastTime);
            data.Start = DateTime.Now - TimeSpan.FromSeconds(seconds);
            data.End = DateTime.Now;

            data.IntervalSeconds = 10;
            data.CountUser = CountUser;
            data.Statt = Statt;
            data.Autorefresh = Autorefresh;
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