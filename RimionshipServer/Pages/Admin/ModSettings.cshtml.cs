using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Services;
namespace RimionshipServer.Pages.Admin
{
    public class ModSettings : PageModel
    {
        private ConfigurationService _configurationService;

        [BindProperty(SupportsGet = true)]
        public IEnumerable<AllowedMod> AllowedMods { get; set; }

        [BindProperty(SupportsGet = true)]
        public byte MaximumLoadOrder { get; set; }

        public ModSettings(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _configurationService.GetAllowedModsAsync();
            AllowedMods = await _configurationService.GetAllowedModsWithOrderAsync()
                                                     .OrderBy(x => x.LoadOrder)
                                                     .ToListAsync();
            
            MaximumLoadOrder = Math.Max((byte) 1, AllowedMods.Max(x => x.LoadOrder));
            return Page();
        }

        public async Task<IActionResult> OnPostMotdAsync(string motd)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> OnPostDeleteModAsync(string packageId, ulong steamId)
        {
            await _configurationService.RemoveAllowedModAsync(packageId, steamId);
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostEditOrderAsync(byte loadOrder, byte originalLoadOrder)
        {
            await _configurationService.EditModOrder(loadOrder, originalLoadOrder);
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostMod(string packageId, ulong steamId, byte loadOrder)
        {
            await _configurationService.AddAllowedMod(new AllowedMod{
                                                                        SteamId   = steamId,
                                                                        PackageId = packageId,
                                                                        LoadOrder = Math.Max((byte) 1, loadOrder)
                                                                    });
            return RedirectToPage("/Admin/ModSettings");
        }
    }
}