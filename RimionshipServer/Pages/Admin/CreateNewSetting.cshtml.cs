﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Services;

namespace RimionshipServer.Pages.Admin
{
    public class CreateNewSetting : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public MiscSettings.Settings Settings { get; set; } = null!;

        private readonly RimionDbContext _dbContext;
        private readonly SettingService _settingService;
        public CreateNewSetting(RimionDbContext dbContext, SettingService settingService)
        {
            _dbContext = dbContext;
            _settingService = settingService;
        }

        public async Task<IActionResult> OnGetAsync(int? settingId)
        {
            if (settingId is not null && await _dbContext.Settings.AnyAsync(x => x.Id == settingId))
            {
                Settings = await _dbContext.Settings
                                           .Include(x => x.Punishment)
                                           .Include(x => x.Rising)
                                           .Include(x => x.Traits)
                                           .FirstAsync(x => x.Id == settingId);
                return Page();
            }
            Settings = new MiscSettings.Settings
            {
                Punishment = new(),
                Rising = new(),
                Traits = new(),
            };
            return Page();
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (Settings.Id == 0)
                _dbContext.Settings.Add(Settings);
            else
                _dbContext.Settings.Update(Settings);

            await _dbContext.SaveChangesAsync();
            await _settingService.ReloadSetting(_dbContext);
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (Settings.Id != 1)
            {
                _dbContext.Settings.Remove(Settings);
                await _dbContext.SaveChangesAsync();
            }
            _ = GrpcService.ToggleResetEvent();
            return RedirectToPage("/Admin/ModSettings");
        }
    }
}