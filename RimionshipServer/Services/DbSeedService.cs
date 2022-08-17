using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;

namespace RimionshipServer.Services
{
    /// <summary>
    /// Used to seed the database on initial startup.
    /// An own class to avoid cluttering the DbContext and allow injection of IOptions.
    /// </summary>
    public class DbSeedService
    {
        private readonly RimionDbContext db;

        public DbSeedService(RimionDbContext db)
        {
            this.db = db;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            await SeedAllowedModsAsync(cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedAllowedModsAsync(CancellationToken cancellationToken = default)
        {
            if (await db.AllowedMods.AnyAsync(cancellationToken))
                return;

            db.AllowedMods.AddRange(
                new AllowedMod { PackageId = "brrainz.harmony", SteamId = 2009463077 },
                new AllowedMod { PackageId = "ludeon.rimworld", SteamId = 0 },
                new AllowedMod { PackageId = "unlimitedhugs.hugslib", SteamId = 818773962 },
                new AllowedMod { PackageId = "brrainz.rimionship", SteamId = 2834585352 },
                new AllowedMod { PackageId = "mastertea.randomplus", SteamId = 1434137894 }, // remove later
                new AllowedMod { PackageId = "automatic.bionicicons", SteamId = 1677616980 },
                new AllowedMod { PackageId = "brrainz.cameraplus", SteamId = 867467808 },
                new AllowedMod { PackageId = "jaxe.rimhud", SteamId = 1508850027 },
                new AllowedMod { PackageId = "tiagocc0.colorblindminerals", SteamId = 1424338139 },
                new AllowedMod { PackageId = "dubwise.dubsmintmenus", SteamId = 1446523594 },
                new AllowedMod { PackageId = "dubwise.dubsmintminimap", SteamId = 1662119905 },
                new AllowedMod { PackageId = "fluffy.followme", SteamId = 715759739 },
                new AllowedMod { PackageId = "falconne.heatmap", SteamId = 947972722 },
                new AllowedMod { PackageId = "krafs.levelup", SteamId = 1701592470 },
                new AllowedMod { PackageId = "fluffy.medicaltab", SteamId = 715565817 },
                new AllowedMod { PackageId = "fluffy.musicmanager", SteamId = 2229205672 },
                new AllowedMod { PackageId = "peppsen.pmusic", SteamId = 725130005 },
                new AllowedMod { PackageId = "legodude17.qualcolor", SteamId = 2420141361 },
                new AllowedMod { PackageId = "automatic.recipeicons", SteamId = 1616643195 },
                new AllowedMod { PackageId = "targhetti.showdrafteesweapon", SteamId = 1690978457 },
                new AllowedMod { PackageId = "crashm.colorcodedmoodbar.11", SteamId = 2006605356 },
                new AllowedMod { PackageId = "mlie.silentdoors", SteamId = 2012447929 },
                new AllowedMod { PackageId = "heye.twitchchat", SteamId = 2075845400 },
                new AllowedMod { PackageId = "vanillaexpanded.vhe", SteamId = 1888705256 },
                new AllowedMod { PackageId = "bodlosh.weaponstats", SteamId = 974066449 },
                new AllowedMod { PackageId = "odeum.wmbp", SteamId = 2314407956 },
                new AllowedMod { PackageId = "com.github.alandariva.moreplanning", SteamId = 2551225702 },
                new AllowedMod { PackageId = "showhair.kv.rw", SteamId = 1180826364 });
        }
    }
}
