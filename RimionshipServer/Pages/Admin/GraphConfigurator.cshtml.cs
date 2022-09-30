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

        public static readonly Dictionary<string, string> StatsNames = new()
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

        public record UserNameId(string Name, string Id);

        public bool Done { get; set; }

        public string AccessCode { get; set; } = null!;

        public string LastTime { get; set; } = null!;
        public List<SelectListItem> LastTimeListItems { get; set; } = null!;

        public List<SelectListItem> StattSelectListItems { get; set; } = null!;
        public string Statt { get; set; } = null!;
        public List<UserNameId> UserSelectListItems { get; set; } = null!;
        public bool[] AllUserSelects { get; set; } = null!;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int IntervalSeconds { get; set; }
        public int CountUser { get; set; }
        public bool Autorefresh { get; set; }
        public List<GraphData> AllGraphs { get; set; } = null!;
        public bool[] AllGraphsSelects { get; set; } = null!;
        public List<GraphRotationData> AllRotations { get; set; } = null!;

        public static string StatsName(string key) => StatsNames.ContainsKey(key) ? StatsNames[key] : key;
        public static string NowNoSeconds() => DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        public async Task<IActionResult> OnGet()
        {
            StattSelectListItems = Stats.FieldNames.Select(x => new SelectListItem(StatsName(x), x)).ToList();
            LastTimeListItems = new List<SelectListItem>{
                                                            new ("1", (1     * 60).ToString()),
                                                            new ("5", (5     * 60).ToString()),
                                                            new ("10", (10   * 60).ToString()),
                                                            new ("30", (30   * 60).ToString()),
                                                            new ("60", (60   * 60).ToString()),
                                                            new ("120", (120 * 60).ToString()),
                                                            new ("240", (240 * 60).ToString()),
                                                            new ("All", (365 * 24 * 60 * 60).ToString())
                                                        };
            await FillComplexArrayFields();
            AllGraphsSelects = new bool[AllGraphs.Count];
            AllUserSelects = new bool[UserSelectListItems.Count];
            Array.Fill(AllGraphsSelects, false);
            Array.Fill(AllUserSelects, false);
            CountUser = 10;
            IntervalSeconds = 10;
            Autorefresh = true;
            return Page();
        }

        [NonAction]
        private async Task FillComplexArrayFields()
        {
            UserSelectListItems = await _dbContext.Users.AsNoTrackingWithIdentityResolution().Select(x => new UserNameId(x.UserName, x.Id)).ToListAsync();
            UserSelectListItems.Sort((u1, u2) => u1.Name.ToLower().CompareTo(u2.Name.ToLower()));
            AllGraphs = await _dbContext.GraphData.AsNoTrackingWithIdentityResolution().Include(x => x.UsersReference).ToListAsync();
            AllRotations = await _dbContext.GraphRotationData.AsNoTrackingWithIdentityResolution().Include(x => x.ToRotate).ToListAsync();
        }

        public Task<IActionResult> OnPostCreateTop10WealthAsync()
        {
            return CreateTop10(nameof(Stats.Wealth));
        }

        public Task<IActionResult> OnPostCreateTop10Async()
        {
            return CreateTop10(Statt);
        }

        public async Task<IActionResult> OnPostCreateAdvancedAsync()
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
            graphData.UsersReference = new List<RimionUser>();
            await _dbContext.SaveChangesAsync();

            await FillComplexArrayFields();
            var ids = new List<string>();
            for (int index = 0; index < AllUserSelects.Length; index++)
                if (AllUserSelects[index])
                    ids.Add(UserSelectListItems[index].Id);

            ((List<RimionUser>)graphData.UsersReference).AddRange(await _dbContext.Users
                                                                                     .Where(x => ids.Contains(x.Id))
                                                                                     .ToArrayAsync());
            //FFS!
            graphData.Start = Start - TimeSpan.FromHours(2);
            graphData.End = End - TimeSpan.FromHours(2);
            return await CreateAsync(graphData);
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
            var seconds = int.Parse(LastTime);
            graphData.Start = DateTime.Now - TimeSpan.FromSeconds(seconds);
            graphData.End = DateTime.Now;
            graphData.CountUser = CountUser;
            return await CreateAsync(graphData);
        }

        [NonAction]
        public async Task<IActionResult> CreateAsync(GraphData data)
        {
            data.IntervalSeconds = IntervalSeconds;
            data.Statt = Statt;
            data.Autorefresh = Autorefresh;
            data.Accesscode = AccessCode;

            if (data.IntervalSeconds == 0)
            {
                ModelState.AddModelError("AccessCode", "IntervalSeconds may not be 0!");
                return await OnGet();
            }
            if (data.End < data.Start)
            {
                ModelState.AddModelError("AccessCode", "data.End < data.Start");
                return await OnGet();
            }
            if (data.CountUser is 0 or null && data.UsersReference.Count() is 0)
            {
                ModelState.AddModelError("AccessCode", "No User Selected");
                return await OnGet();
            }
            if (data.Id == 0)
                _dbContext.GraphData.Add(data);
            else
                _dbContext.GraphData.Update(data);

            await _dbContext.SaveChangesAsync();
            Done = true;
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteGraphAsync(int id)
        {
            var graph = await _dbContext.GraphData.FindAsync(id);
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));
            _dbContext.GraphData.Remove(graph);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/GraphConfigurator");
        }

        public async Task<IActionResult> OnPostCreateRotationAsync()
        {
            if (AccessCode is null || string.IsNullOrWhiteSpace(AccessCode) || HttpUtility.UrlEncode(AccessCode) != AccessCode)
            {
                ModelState.AddModelError("AccessCode", "Name must be set and can not contain forbidden URL symbols (i.E. / ? = < > etc.)");
                return await OnGet();
            }
            var rotation = await _dbContext.GraphRotationData
                                           .Include(x => x.ToRotate)
                                           .Where(x => x.RotationName == AccessCode)
                                           .FirstOrDefaultAsync() ?? new GraphRotationData();
            await FillComplexArrayFields();
            rotation.RotationName = AccessCode;
            rotation.TimeToDisplay = IntervalSeconds;
            rotation.ToRotate = new List<GraphData>();
            await _dbContext.SaveChangesAsync();

            for (int index = 0; index < AllGraphs.Count; index++)
                if (AllGraphsSelects[index])
                    ((List<GraphData>)rotation.ToRotate).Add(await _dbContext.GraphData.FindAsync(AllGraphs[index].Id) ?? throw new ArgumentNullException());

            if (rotation.ToRotate.Count() is 0)
            {
                ModelState.AddModelError("AccessCode", "No Graph Selected");
                return await OnGet();
            }

            if (rotation.Id is 0)
                _dbContext.GraphRotationData.Add(rotation);
            else
                _dbContext.GraphRotationData.Update(rotation);

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Admin/GraphConfigurator");
        }

        public async Task<IActionResult> OnPostDeleteRotationAsync(int id)
        {
            var graph = await _dbContext.GraphRotationData.FindAsync(id);
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));
            _dbContext.GraphRotationData.Remove(graph);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/GraphConfigurator");
        }
    }
}