using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;

namespace RimionshipServer.Pages.Api;

public class SimpleGraph : PageModel
{
    public static readonly Color[] Colors ={
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
        _dbContext                 =  dbContext;
    }

    [BindProperty(SupportsGet = true)]
    public string GraphName { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string[] Labels { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public IEnumerable<Dataset> Datasets { get; set; } = null!;

    private GraphData _graphData { get; set; } = null!;

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

        DateTimeOffset start;
        DateTimeOffset end;
        if (_graphData.Autorefresh)
        {
            var diff         = _graphData.End - _graphData.Start;
            start = DateTime.Now - diff;
            end   = DateTime.Now;
        }
        else
        {
            start = _graphData.Start;
            end   = _graphData.End;
        }
        ((List<DateTimeOffset> Timestamp, List<object> Values), string UserName)[] tasks;
        
        if (_graphData.CountUser is not null)
        {
            tasks = await Task.WhenAll((await Stats.GetTopXNotBannedUserFromDynamicCache(_graphData.Statt, _graphData.CountUser.Value, _dbContext))
                                      .Select(async userId =>
                                                  ((await _dbContext.FetchDataVerticalAsync(start, end, _graphData.IntervalSeconds, _graphData.Statt, userId.Id), userId.UserName)
                                                  )));
        }
        else
        {
            tasks = await Task.WhenAll(_graphData.UsersReference
                                                 .Where(x => !x.WasBanned)
                                                 .Select(async userId =>
                                                             ((await _dbContext.FetchDataVerticalAsync(start, end, _graphData.IntervalSeconds, _graphData.Statt, userId.Id)), userId.UserName)
                                                        ));
        }

        var datasetRecords = new Dictionary<string, List <DataEntry>>();
        foreach (var perUser in tasks) //per user
        {
            var dataRecords = new List<DataEntry>();
            for (var index = 0; index < perUser.Item1.Timestamp.Count; index++)
            {
                var f   = perUser.Item1.Timestamp[index].ToUnixTimeMilliseconds();
                var obj = perUser.Item1.Values[index].ToString()!;
                if (obj is null or "0" or "")
                    continue;
                dataRecords.Add(new DataEntry(f, obj));
            }
            datasetRecords.Add(perUser.UserName, dataRecords);
        }
        
        var datasets = datasetRecords
                      .Where(x => x.Value is not null && x.Value.Count > 0)
                      .OrderByDescending(x => float.Parse(x.Value.Where(d => d.y is not null && d.y != String.Empty).MaxBy(d => d.x)!.y))
                      .Select((datasetRecord, index) => new Dataset(datasetRecord.Key, Colors[index], datasetRecord.Value));

        Datasets  = datasets;
        GraphName = _graphData.Accesscode;
        return Page();
    }
    
    public async Task<IActionResult> OnGetEmbedAsync(string statt, string username, string width, string height)
    {
        ViewData["embed"]  = true;
        ViewData["width"]  = width;
        ViewData["height"] = height;
        var user        = await _dbContext.Users.Where(x => x.UserName == username).FirstAsync();
        var perUser     = await _dbContext.FetchDataVerticalAsync(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, 10, statt, user.Id);
        var dataRecords = new List<DataEntry>();
        for (var index = 0; index < perUser.Timestamp.Count; index++)
        {
            var f   = perUser.Timestamp[index].ToUnixTimeMilliseconds();
            var obj = perUser.Values[index].ToString()!;
            if (obj is null or "0" or "")
                continue;
            dataRecords.Add(new DataEntry(f, obj));
        }
        Datasets = new[]{new Dataset(username, Colors[0], dataRecords)};
        return Page();
    }
}