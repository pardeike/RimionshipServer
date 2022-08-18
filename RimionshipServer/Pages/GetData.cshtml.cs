using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RimionshipServer.Data.Detailed;
using RimionshipServer.Services;

namespace RimionshipServer.Pages;

//TODO: SAMPLE!

[AllowAnonymous]
public class GetData : PageModel
{
    private readonly GameDataService _gameDataService;
    
    public GetData(GameDataService gameDataService)
    {
        _gameDataService = gameDataService;
    }
    

    public async Task<IActionResult> OnGetAsync(string userId, int stat, CancellationToken cancellationToken)
    {
        var colonyStat = (ColonyStat) stat;
        return new JsonResult(await _gameDataService.GetIntDataForTimeSpan(userId, 
                                                                           colonyStat,
                                                                           DateTime.Now - TimeSpan.FromMinutes(5),
                                                                           DateTime.Now,
                                                                           TimeSpan.FromMinutes(0.5f),
                                                                           cancellationToken));
    }
}