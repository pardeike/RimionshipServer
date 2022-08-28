using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
namespace RimionshipServer.Services
{
    public class SettingService
    {
        private MiscSettings.Settings?         _activeSetting;

        public async Task<MiscSettings.Settings> GetActiveSetting(RimionDbContext dbContext)
        {
            if (_activeSetting is not null)
                return _activeSetting;

            return await dbContext.Settings
                                  .Include(x => x.Punishment)
                                  .Include(x => x.Rising)
                                  .Include(x => x.Traits)
                                  .FirstAsync(x => x.Id == 1);
        }
        
        public async Task SelectActiveSetting(RimionDbContext dbContext, int settingId)
        {
            _activeSetting = await dbContext.Settings
                                            .AsNoTrackingWithIdentityResolution()
                                            .Include(x => x.Punishment)
                                            .Include(x => x.Rising)
                                            .Include(x => x.Traits)
                                            .FirstOrDefaultAsync(x => x.Id == settingId) 
                         ?? await dbContext.Settings
                                           .AsNoTrackingWithIdentityResolution()
                                           .Include(x => x.Punishment)
                                           .Include(x => x.Rising)
                                           .Include(x => x.Traits)
                                           .FirstAsync(x => x.Id          == 1);
        }
    }
}