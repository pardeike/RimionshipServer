using System.Buffers;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Pages.Api;
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
        
        [BindProperty(SupportsGet = true)]
        public byte PlannedStartHour { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public byte PlannedStartMinute { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public byte GameState { get; set; }
        
        [BindProperty]
        public IFormFile Upload { get; set; }

        [BindProperty(SupportsGet = true)]
        public IEnumerable<SelectListItem> SaveFileSelect { get; set; }
        
        [BindProperty]
        public int SaveFileId { get; set; }
        
        public async Task<IActionResult> OnPostFileSelectAsync()
        {
            SaveFilePageModel.SafeFile = await _dbContext.SaveFiles.FindAsync(SaveFileId);
            return RedirectToPage("/Admin/ModSettings");
        }
        
        public async Task<IActionResult> OnPostUploadAsync()
        {
            using var       checksum = MD5.Create();
            await using var uploaded = Upload.OpenReadStream();
            var             filename = WebUtility.UrlEncode(Upload.FileName.Trim());
            if (!filename.EndsWith(".rws"))
            {
                ModelState.AddModelError(nameof(Upload), "Can only upload .rws save files!");
                return await OnGetAsync();
            }
            var             rent     = ArrayPool<byte>.Shared.Rent((int) uploaded.Length);
            try
            {
                await using var memstream = new MemoryStream(rent, 0, (int) uploaded.Length);
                await uploaded.CopyToAsync(memstream);
                memstream.Position = 0;
                var hashTask = await checksum.ComputeHashAsync(memstream);
                memstream.Position = 0;
                await using var compressed = await Compress(memstream);
                var             md5B64     = Convert.ToBase64String(hashTask);
                var safeFile = new SaveFile{
                                               File = compressed.ToArray(),
                                               MD5  = md5B64,
                                               Name = filename
                                           };
                _dbContext.SaveFiles.Add(safeFile);
                await _dbContext.SaveChangesAsync();
                return RedirectToPage("/Admin/ModSettings");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }

        private static async Task<MemoryStream> Compress(Stream input)
        {
            var outputStream = new MemoryStream();
            await using GZipStream gzip         = new (outputStream, CompressionLevel.SmallestSize);
            await input.CopyToAsync(gzip);
            await gzip.FlushAsync();
            return outputStream;
        }
        
        public ModSettings(ConfigurationService configurationService, RimionDbContext dbContext, SettingService settingService)
        {
            _configurationService = configurationService;
            _dbContext            = dbContext;
            _settingService       = settingService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _configurationService.GetAllowedModsAsync();
            AllowedMods = await _configurationService.GetAllowedModsWithOrderAsync()
                                                     .OrderBy(x => x.LoadOrder)
                                                     .ToListAsync(HttpContext.RequestAborted);

            MaximumLoadOrder = Math.Max((byte) 1, AllowedMods.Max(x => x.LoadOrder));

            var settings = await _dbContext.Settings.AsNoTrackingWithIdentityResolution().ToListAsync(HttpContext.RequestAborted);

            SettingsSelect = settings
                            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                            .ToList();
            
            ActiveSetting    = (await _settingService.GetActiveSetting(_dbContext, HttpContext.RequestAborted)).Name;
            SettingsSelect   = SettingsSelect.Append(new SelectListItem("Create New", "cn"));
            Motd             = await _dbContext.GetMotdAsync(HttpContext.RequestAborted);
            var gameState = await _dbContext.GetGameStateAsync(HttpContext.RequestAborted);
            PlannedStartHour   = (byte) gameState.PlannedStartHour;
            PlannedStartMinute = (byte) gameState.PlannedStartMinute;
            GameState          = (byte) gameState.GameState;
            SaveFileSelect     = (await _dbContext.SaveFiles
                                                  .AsNoTracking()
                                                  .Select(x => new {x.Name, x.Id})
                                                  .ToArrayAsync())
                                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                                .Prepend(new SelectListItem("None", string.Empty, true))
                                .ToArray();
            return Page();
        }

        public async Task<IActionResult> OnPostMotdAsync(string? motd)
        {
            await _dbContext.SetMotdAsync(motd);
            _ = GrpcService.ToggleResetEvent();
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

        public async Task<IActionResult> OnPostModAsync(string packageId, ulong steamId, byte loadOrder)
        {
            await _configurationService.AddAllowedMod(new AllowedMod{
                                                                        SteamId   = steamId,
                                                                        PackageId = packageId,
                                                                        LoadOrder = Math.Max((byte) 1, loadOrder)
                                                                    });
            return RedirectToPage("/Admin/ModSettings");
        }
        
        public async Task<IActionResult> OnPostSettingAsync()
        {
            if (ActiveSetting == "cn")
            {
                return RedirectToPage("/Admin/CreateNewSetting");
            }
            await _settingService.SelectActiveSetting(_dbContext,int.Parse(ActiveSetting));
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }
        
        public async Task<IActionResult> OnPostGameAsync()
        {
            await _dbContext.SetGameStateAsync(GameState, PlannedStartHour, PlannedStartMinute);
            return RedirectToPage("/Admin/ModSettings");
        }
        
        public async Task<IActionResult> OnPostDeleteAllAsync()
        {
            if (((State.Types.Game) (await _dbContext.GetGameStateAsync()).GameState) != State.Types.Game.Completed)
                return Forbid();
            
            _dbContext.HistoryStats.RemoveRange(await _dbContext.HistoryStats.AsNoTrackingWithIdentityResolution().ToArrayAsync());
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/ModSettings");
        }
    }
}