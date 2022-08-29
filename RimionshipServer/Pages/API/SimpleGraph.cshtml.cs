using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data;
namespace RimionshipServer.Pages.Api;

public class SimpleGraph : PageModel
{
    public record Dataset(string label, string backgroundColor, string borderColor, float[] data);
    
    public SimpleGraph(RimionDbContext dbContext)
    {
        _dbContext         = dbContext;
    }

    [BindProperty(SupportsGet = true)]
    public string[] Labels { get; set; }
        
    [BindProperty(SupportsGet = true)]
    public Dataset[] Datasets { get; set; }

    private readonly RimionDbContext _dbContext;

    public async Task<IActionResult> OnGet(string id, string secret)
    {
        if (id != "foo" || secret != "bar")
        {
            return NotFound();
        }
        Labels = new[]{
                          "January",
                          "February",
                          "March",
                          "April",
                          "May",
                          "June"
                      };
        Datasets = new[]{new Dataset("My First dataset", "rgb(255, 99, 132)", "rgb(255, 99, 132)", new float[]{0, 10, 5, 2, 20, 30, 45})};
        return Page();
    }
}