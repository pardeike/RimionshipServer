using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;

namespace RimionshipServer.API
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly RimionDbContext db;

        public StatsController(RimionDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> ListEntries()
            => Ok(await db.HistoryStats.Include(u => u.User).OrderByDescending(a => a.Id).ToListAsync());
    }
}
