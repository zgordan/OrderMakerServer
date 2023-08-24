using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Account
{
    public class ReLoginModel : PageModel
    {

        private readonly UserManager<WebAppUser> userManager;

        public ReLoginModel(UserManager<WebAppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            WebAppUser user = await userManager.GetUserAsync(HttpContext.User);
            if (user == null) { return RedirectToPage("/Identity/Account/Login"); }

            await userManager.RemoveClaimAsync(user, new Claim("revoke", "false"));
            await HttpContext.RefreshLoginAsync();

            var targetUrl = $"Referer: {HttpContext.Request.Scheme}://{HttpContext.Request.Host}{returnUrl}";

            return Redirect(targetUrl);

        }
    }
}
