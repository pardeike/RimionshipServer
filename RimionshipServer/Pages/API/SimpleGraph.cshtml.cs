using System.Drawing;
using System.Runtime.CompilerServices;
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

    public async Task<IActionResult> OnGet(string id, string secret)
    {
        // _graphData = await _dbContext.GraphData
        //                              .Where(x => x.Accesscode == id)
        //                              .Where(x => x.Secret     == secret)
        //                              .FirstOrDefaultAsync();
        // if (_graphData is null)
        // {
        //     return NotFound();
        // }

        var users = await _dbContext.Users
                                    .OrderBy(x => x.LatestStats)
                                    .Select(x => x.Id)
                                    .Where(x => _dbContext.HistoryStats.Where(y => y.UserId == x).Any(y => y.Wealth > 0))
                                    .Take(10)
                                    .ToArrayAsync();
        
        _graphData = new GraphData{
                                      Accesscode      = "foo",
                                      Start           = new DateTimeOffset(DateTime.Now - TimeSpan.FromDays(30)),
                                      End             = new DateTimeOffset(DateTime.Now),
                                      Id              = 0,
                                      IntervalSeconds = 10,
                                      Secret          = "bar",
                                      Statt           = Stats.FieldNames[0],
                                      Users           = users
                                  };

        if (_graphData is null)
        {
            return NotFound();
        }

        var tasks = (await Task.WhenAll(
                                        _graphData.Users
                                                  .Select(userId =>
                                                              _dbContext.FetchDataVerticalAsync(_graphData.Start, _graphData.End, _graphData.IntervalSeconds, _graphData.Statt, userId)
                                                         )
                                       ))
           .ToList();

        var datasetRecords = new List<List<Data>>();
        foreach (var perUser in tasks) //per user
        {
            var dataRecords = new List<Data>();
            for (var index = 0; index < perUser.Timestamp.Count; index++)
            {
                var f   = perUser.Timestamp[index].ToUnixTimeMilliseconds();
                var obj = perUser.Values[index].ToString()!;
                dataRecords.Add(new Data(f, obj));
            }
            datasetRecords.Add(dataRecords);
        }
        
        var datasets = datasetRecords
                      .Where(x => x is not null && x.Count > 0)
                      .OrderByDescending(x => float.Parse(x.Where(d => d.y is not null && d.y != String.Empty).MaxBy(d => d.x)!.y))
                      .Select(async (datasetRecord, index) => new Dataset(await _dbContext.Users.Where(x => x.Id == _graphData.Users[index]).Select(x => x.UserName).FirstAsync(), Colors[index], datasetRecord));

        Datasets  = await Task.WhenAll(datasets);
        GraphName = "Test";
        return Page();
    }
}