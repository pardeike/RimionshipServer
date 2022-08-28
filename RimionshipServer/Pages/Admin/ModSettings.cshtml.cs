using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
using RimionshipServer.Services;
namespace RimionshipServer.Pages.Admin
{
    public class ModSettings : PageModel
    {
        private ConfigurationService _configurationService;
        private SettingService       _settingService;
        private RimionDbContext      _dbContext;
        
        [BindProperty(SupportsGet = true)]
        public string Motd { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public IEnumerable<AllowedMod> AllowedMods { get; set; }

        [BindProperty(SupportsGet = true)]
        public byte MaximumLoadOrder { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string ActiveSetting { get; set; }

        [BindProperty(SupportsGet = true)]
        public IEnumerable<SelectListItem> SettingsSelect { get; set; }
        
        public ModSettings(ConfigurationService configurationService, RimionDbContext dbContext, SettingService settingService)
        {
            _configurationService = configurationService;
            _dbContext            = dbContext;
            _settingService  = settingService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _configurationService.GetAllowedModsAsync();
            AllowedMods = await _configurationService.GetAllowedModsWithOrderAsync()
                                                     .OrderBy(x => x.LoadOrder)
                                                     .ToListAsync();

            MaximumLoadOrder = Math.Max((byte) 1, AllowedMods.Max(x => x.LoadOrder));

            var settings = await _dbContext.Settings.AsNoTrackingWithIdentityResolution().ToListAsync();

            SettingsSelect = settings
                            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                            .ToList();
            
            ActiveSetting  = (await _settingService.GetActiveSetting(_dbContext)).Name;
            SettingsSelect = SettingsSelect.Append(new SelectListItem("Create New", "cn"));
            Motd           = await _dbContext.GetMotdAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostMotdAsync(string? motd)
        {
            await _dbContext.SetMotdAsync(motd);
            return RedirectToPage("/Admin/ModSettings");
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
        
        public async Task<IActionResult> OnPostSetting()
        {
            if (ActiveSetting == "cn")
            {
                return RedirectToPage("/Admin/CreateNewSetting");
            }
            await _settingService.SelectActiveSetting(_dbContext,int.Parse(ActiveSetting));
            return RedirectToPage("/Admin/ModSettings");
        }
    }
}