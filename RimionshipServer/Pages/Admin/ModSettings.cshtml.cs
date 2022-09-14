using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Pages.Api;
using RimionshipServer.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
namespace RimionshipServer.Pages.Admin
{
    public class ModSettings : PageModel
    {
        private readonly ConfigurationService _configurationService;
        private readonly SettingService       _settingService;
        private readonly RimionDbContext      _dbContext;

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Broadcast Message: ")]
        public string Motd { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public IEnumerable<AllowedMod> AllowedMods { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public byte MaximumLoadOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Einstellungen: ")]
        public string ActiveSetting { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public IEnumerable<SelectListItem> SettingsSelect { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public byte PlannedStartHour { get; set; }

        [BindProperty(SupportsGet = true)]
        public byte PlannedStartMinute { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Spiel Status: ")]
        public byte GameState { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public IEnumerable<SelectListItem> SaveFileSelect { get; set; } = null!;

        [BindProperty]
        public int SaveFileId { get; set; }
        
        [BindProperty]
        public string DownloadURI { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Pawns zum Start:")]
        public int StartingPawns { get; set; }

        public async Task<IActionResult> OnPostFileSelectAsync()
        {
            SaveFilePageModel.SafeFile = await _dbContext.SaveFiles.FindAsync(SaveFileId);
            Debug.Assert(SaveFilePageModel.SafeFile is not null);
            await _dbContext.SetSaveSettingsAsync("http://" + DownloadURI, SaveFilePageModel.SafeFile, StartingPawns);
            return RedirectToPage("/Admin/ModSettings");
        }

        private static async Task ReplaceTicks(Stream uploaded, MemoryStream memstream)
        {
            await using var br = new BufferedStream(uploaded);
            using var       sr = new StreamReader(br);
            await using var sw = new StreamWriter(memstream, Encoding.UTF8, -1, true);
            while (true)
            {
                var line = await sr.ReadLineAsync();
                if (line == null)
                    break;
                if (line.Contains("<ticksGame>"))
                {
                    line = "<ticksGame>0</ticksGame>";
                }
                await sw.WriteLineAsync(line);
            }
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            using var       checksum = MD5.Create();
            await using var uploaded = Upload.OpenReadStream();
            var rent =  ArrayPool<byte>.Shared.Rent((int) Upload.Length);
            try
            {
                await using var memstream = new MemoryStream(rent, 0, (int) Upload.Length -1);
                await ReplaceTicks(uploaded, memstream);
                var filename  = WebUtility.UrlEncode(Upload.FileName.Trim());
                if (!filename.EndsWith(".rws"))
                {
                    ModelState.AddModelError(nameof(Upload), "Can only upload .rws save files!");
                    return await OnGetAsync();
                }
                var oldFile = await _dbContext.SaveFiles.Where(x => x.Name == filename).FirstOrDefaultAsync();
                var hash    = await checksum.ComputeHashAsync(memstream);
                var md5B64  = Convert.ToHexString(hash).ToLower();
                if (oldFile?.MD5 == md5B64)
                {
                    return RedirectToPage("/Admin/ModSettings");
                }
                memstream.Position = 0;
                await using var compressedStream = new MemoryStream();
                await Compress(memstream, compressedStream);
                if (oldFile is null)
                {
                    var safeFile = new SaveFile{
                                                   File = compressedStream.ToArray(),
                                                   MD5  = md5B64,
                                                   Name = filename
                                               };

                    _dbContext.SaveFiles.Add(safeFile);
                }
                else
                {
                
                    oldFile.File = compressedStream.ToArray();
                    oldFile.MD5  = md5B64;
                    _dbContext.SaveFiles.Update(oldFile);
                }
                await _dbContext.SaveChangesAsync();
                return RedirectToPage("/Admin/ModSettings");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }

        private static async Task Compress(Stream input, MemoryStream output)
        {
            await using GZipStream gzip = new(output, CompressionLevel.SmallestSize);
            await input.CopyToAsync(gzip);
            await gzip.FlushAsync();
        }

        public ModSettings(ConfigurationService configurationService, RimionDbContext dbContext, SettingService settingService)
        {
            _configurationService = configurationService;
            _dbContext = dbContext;
            _settingService = settingService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _configurationService.GetAllowedModsAsync();
            AllowedMods = await _configurationService.GetAllowedModsWithOrderAsync()
                                                     .OrderBy(x => x.LoadOrder)
                                                     .ToListAsync(HttpContext.RequestAborted);

            MaximumLoadOrder = Math.Max((byte)1, AllowedMods.Max(x => x.LoadOrder));

            var settings = await _dbContext.Settings.AsNoTrackingWithIdentityResolution().ToListAsync(HttpContext.RequestAborted);

            SettingsSelect = settings
                            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                            .ToList();

            ActiveSetting = (await _settingService.GetActiveSetting(_dbContext, HttpContext.RequestAborted)).Name;
            SettingsSelect = SettingsSelect.Append(new SelectListItem("Create New", "cn"));
            Motd = await _dbContext.GetMotdAsync(HttpContext.RequestAborted);
            var gameState = await _dbContext.GetGameStateAsync(HttpContext.RequestAborted);
            PlannedStartHour = (byte)gameState.PlannedStartHour;
            PlannedStartMinute = (byte)gameState.PlannedStartMinute;
            GameState = (byte)gameState.GameState;
            SaveFileSelect = (await _dbContext.SaveFiles
                                                  .AsNoTracking()
                                                  .Select(x => new { x.Name, x.Id })
                                                  .ToArrayAsync())
                                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                                .Prepend(new SelectListItem("None", string.Empty, true))
                                .ToArray();
            StartingPawns = (await _dbContext.GetSaveSettingsAsync()).CountColonists;
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
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostEditOrderAsync(byte loadOrder, byte originalLoadOrder)
        {
            await _configurationService.EditModOrder(loadOrder, originalLoadOrder);
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostModAsync(string packageId, ulong steamId, byte loadOrder)
        {
            await _configurationService.AddAllowedMod(new AllowedMod
            {
                SteamId = steamId,
                PackageId = packageId,
                LoadOrder = Math.Max((byte)1, loadOrder)
            });
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostSettingAsync()
        {
            if (ActiveSetting == "cn")
            {
                return RedirectToPage("/Admin/CreateNewSetting");
            }
            await _settingService.SelectActiveSetting(_dbContext, int.Parse(ActiveSetting));
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostGameAsync()
        {
            await _dbContext.SetGameStateAsync(GameState, PlannedStartHour, PlannedStartMinute);
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostDeleteAllAsync()
        {
            if (((State.Types.Game)(await _dbContext.GetGameStateAsync()).GameState) != State.Types.Game.Completed)
                return Forbid();

            _dbContext.HistoryStats.RemoveRange(await _dbContext.HistoryStats.AsNoTrackingWithIdentityResolution().ToArrayAsync());
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/ModSettings");
        }
    }
}