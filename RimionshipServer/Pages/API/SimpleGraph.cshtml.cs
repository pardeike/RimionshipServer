using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;
namespace RimionshipServer.Pages.Api;

public class SimpleGraph : PageModel
{
    public record Dataset
    {
        public Dataset(string label, Color color, float[] data)
        {
            this.label           = label;
            borderColor     = HexConverter(color);
            this.data            = data;
            backgroundColor = HexConverter(color);
        }
        
        public string  backgroundColor { get; init; }
        public string  label           { get; init; }
        public string  borderColor     { get; init; }
        public float[] data            { get; init; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
    
    public SimpleGraph(RimionDbContext dbContext)
    {
        _dbContext         = dbContext;
    }

    [BindProperty(SupportsGet = true)]
    public string GraphName { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string[] Labels { get; set; }
        
    [BindProperty(SupportsGet = true)]
    public Dataset[] Datasets { get; set; }

    private readonly RimionDbContext _dbContext;

    public async Task<IActionResult> OnGet(string id, string secret)
    {
        //TODO: Verify id with database secret for super simple "auth"
        if (id != "foo" || secret != "bar")
        {
            return NotFound();
        }
        //TODO: Timepoints on x-Axis
        Labels = new[] {
                          "January",
                          "February",
                          "March",
                          "April",
                          "May",
                          "June"
                      };
        //TODO: Datapoints
        Datasets  = new[] {
                             new Dataset("Schemfil - 1020245",                                      Color.Aqua,    new float[]{0, 10, 5, 2, 20, 30, 45}),
                             new Dataset("Brrainz - 1020245",                                       Color.DarkRed, new float[]{0, 70, 0, 42, 10, 3, 60}),
                             new Dataset("SehrLangerName123SehrLangWirklichzulangdigger - 1020245", Color.Green,    new float[]{12, 2, 34, 7, 10, 30, 4})
                         };
        //TODO: Website Title
        GraphName = "Test";
        return Page();
    }
}