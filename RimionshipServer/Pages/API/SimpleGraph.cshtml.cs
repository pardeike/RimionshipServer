using System.Drawing;
using System.Runtime.CompilerServices;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
namespace RimionshipServer.Pages.Api;

public class SimpleGraph : PageModel
{
    public record Data(long x, string y);
    public record Dataset
    {
        public Dataset(string label, Color color, IEnumerable<Data> data)
        {
            this.label      = label;
            borderColor     = HexConverter(color);
            this.data       = data;
            backgroundColor = HexConverter(color);
        }

        public string            backgroundColor { get; init; }
        public string            label           { get; init; }
        public string            borderColor     { get; init; }
        public IEnumerable<Data> data            { get; init; }
        public bool              showLine        { get; init; } = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }


    private static readonly Color[] Colors ={
                                                ColorTranslator.FromHtml("#00429d"),
                                                ColorTranslator.FromHtml("#3c66ad"),
                                                ColorTranslator.FromHtml("#5f8bbc"),
                                                ColorTranslator.FromHtml("#82b2c9"),
                                                ColorTranslator.FromHtml("#abd8d2"),
                                                ColorTranslator.FromHtml("#fbc8b7"),
                                                ColorTranslator.FromHtml("#f79291"),
                                                ColorTranslator.FromHtml("#e45d6f"),
                                                ColorTranslator.FromHtml("#c42a51"),
                                                ColorTranslator.FromHtml("#93003a")
                                             };

    public SimpleGraph(RimionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [BindProperty(SupportsGet = true)]
    public string GraphName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string[] Labels { get; set; }

    [BindProperty(SupportsGet = true)]
    public IEnumerable<Dataset> Datasets { get; set; }

    private GraphData _graphData { get; set; }

    private readonly RimionDbContext _dbContext;

    public async Task<IActionResult> OnGet(string secret)
    {
        var accessEncoded = secret;
        var graphData = await _dbContext.GraphData
                                        .AsNoTrackingWithIdentityResolution()
                                        .Include(x => x.UsersReference)
                                        .Where(x => x.Accesscode == accessEncoded)
                                        .FirstOrDefaultAsync();
        if (graphData is null)
        {
            return NotFound();
        }
        _graphData = graphData;

        var diff     = _graphData.End - _graphData.Start;
        var newStart = DateTime.Now   - diff;
        
        var tasks = (await Task.WhenAll(
                                        _graphData.Users
                                                  .Select(async userId =>
                                                              ((await _dbContext.FetchDataVerticalAsync(newStart, DateTime.Now, _graphData.IntervalSeconds, _graphData.Statt, userId)), userId)
                                                         )
                                       ))
           .ToList();

        var datasetRecords = new Dictionary<string, List <Data>>();
        foreach (var perUser in tasks) //per user
        {
            var dataRecords = new List<Data>();
            for (var index = 0; index < perUser.Item1.Timestamp.Count; index++)
            {
                var f   = perUser.Item1.Timestamp[index].ToUnixTimeMilliseconds();
                var obj = perUser.Item1.Values[index].ToString()!;
                dataRecords.Add(new Data(f, obj));
            }
            datasetRecords.Add(perUser.userId, dataRecords);
        }
        
        var datasets = datasetRecords
                      .Where(x => x.Value is not null && x.Value.Count > 0)
                      .OrderByDescending(x => float.Parse(x.Value.Where(d => d.y is not null && d.y != String.Empty).MaxBy(d => d.x)!.y))
                      .Select(async (datasetRecord, index) => 
                                  new Dataset(await _dbContext.Users.Where(x => x.Id == datasetRecord.Key)
                                                              .Select(x => x.UserName)
                                                              .FirstAsync(), Colors[index], datasetRecord.Value)
                              );

        Datasets  = await Task.WhenAll(datasets);
        GraphName = "Test";
        return Page();
    }
}