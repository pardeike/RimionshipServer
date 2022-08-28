using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Users;
using Serilog;
namespace RimionshipServer.Pages.Admin
{
    public class Manage : PageModel
    {
        public record UsersDTO(bool WasBanned, bool IsSuspicious, string UserName, string Id, IList<string> Role);

        [BindProperty(SupportsGet = true)]
        public IEnumerable<UsersDTO> Users { get; set; } = null!;
        
        [BindProperty(SupportsGet = true)]
        public int Pages { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int pageNo { get; set; }
        
        public const int ElementsPerSite = 25;
        
        private readonly UserManager _userManager;

        public Manage(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Pages = await _userManager.Users.CountAsync(HttpContext.RequestAborted) / ElementsPerSite + 1;
            Users = await Task.WhenAll((await _userManager.Users
                                                          .AsNoTrackingWithIdentityResolution()
                                                          .OrderBy(x => x.UserName)
                                                          .Skip(ElementsPerSite * pageNo)
                                                          .Take(ElementsPerSite)
                                                          .ToListAsync(HttpContext.RequestAborted))
                                      .Select(async x => new UsersDTO(x.WasBanned, x.IsSuspicious, x.UserName, x.Id, await _userManager.GetRolesAsync(x))));
            return Page();
        }

        public async Task<IActionResult> OnPostBanAsync(string id, bool ban)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();
            var toBan = await _userManager.Users.Where(x => x.Id == id).FirstAsync(CancellationToken.None);
            toBan.WasBanned    = ban;
            toBan.IsSuspicious = ban;
            var wasBanned = await _userManager.UpdateAsync(toBan);
            if (wasBanned.Succeeded)
                return RedirectToPage("/Admin/Manage", pageNo);
            
            foreach (var wasBannedError in wasBanned.Errors)
            {
                Log.Error("Error while banning {User}, Error Code: {Code}, ErrorDescription: {Description}", toBan.UserName, wasBannedError.Code, wasBannedError.Description);
                ModelState.AddModelError(wasBannedError.Code, wasBannedError.Description);
            }
            return Page();
        }
        
        public async Task<IActionResult> OnPostPromoteAsync(string id)
        {
            var user  = await _userManager.Users.Where(x => x.Id == id).FirstAsync(CancellationToken.None);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(Roles.Admin))
            {
                await _userManager.RemoveFromRoleAsync(user, Roles.Admin);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, Roles.Admin);
            }

            return RedirectToPage("/Admin/Manage", pageNo);
        }

        public async Task<IActionResult> OnPostSearchAsync(string searchKey)
        {
            Users = await Task.WhenAll((await _userManager.Users
                                                          .AsNoTrackingWithIdentityResolution()
                                                          .Where(x => EF.Functions.Like(x.UserName, $"%{searchKey}%"))
                                                          .Take(ElementsPerSite)
                                                          .ToListAsync(HttpContext.RequestAborted))
                                              .Select(async x => new UsersDTO(x.WasBanned, x.IsSuspicious, x.UserName, x.Id, await _userManager.GetRolesAsync(x))));
            return Page();
        }
    }
}