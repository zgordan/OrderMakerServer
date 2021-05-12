using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Extensions;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Account
{
    public class ReLoginModel : PageModel
    {

        private UserManager<WebAppUser> userManager;

        public ReLoginModel(UserManager<WebAppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            WebAppUser user = await userManager.GetUserAsync(HttpContext.User);
            if (user == null) { return RedirectToPage("/Identity/Account/Login"); }

            await userManager.RemoveClaimAsync(user, new Claim("revoke", "false"));
            var referer = HttpContext.Request.Headers["Referer"].ToString();
            var host = HttpContext.Request.Host.Value;
            await HttpContext.RefreshLoginAsync();
       
            bool isLocalUrl = referer.Contains(host);

            if (isLocalUrl)
            {
                return Redirect(referer);
            }

            return RedirectToPage("/Index");

        }
    }
}
