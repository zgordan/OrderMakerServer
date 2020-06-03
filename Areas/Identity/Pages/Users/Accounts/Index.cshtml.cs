using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly UserHandler _userHandler;
        private readonly RoleManager<WebAppRole> _roleManager;

        public IndexModel(UserHandler userHandler, RoleManager<WebAppRole> roleManager)
        {            
            _userHandler = userHandler;
            _roleManager = roleManager;
        }

        public IList<WebAppPerson> Accounts { get; set; }
        public string SearchText { get; set; }
        public async Task<IActionResult> OnGetAsync(string searchText)
        {
            var query = _userHandler.Users;

            if (searchText != null)
            {
                string normText = searchText.ToUpper();
                query = query.Where(x => x.Title.ToUpper().Contains(normText) ||
                                        x.UserName.ToUpper().Contains(normText) || 
                                        x.Email.ToUpper().Contains(normText) || 
                                        x.TitleGroup.ToUpper().Contains(normText));
                SearchText = searchText;
            }

            Accounts = new List<WebAppPerson>();
            IList<WebAppUser> users = await query.ToListAsync();
            foreach (WebAppUser user in users)
            {
                IList<string> roles = await _userHandler.GetRolesAsync(user);
                Accounts.Add(new WebAppPerson
                {
                    User = user,
                    Role = await _roleManager.FindByNameAsync(roles.Where(x=>!x.ToUpper().Contains("CPQ-")).FirstOrDefault()),
                    MtdPolicy = await _userHandler.GetPolicyForUserAsync(user)
                });
            }

                       
            return Page();
        }
    }

    public class WebAppPerson
    {
        public WebAppUser User { get; set; }
        public WebAppRole Role { get; set; }
        public MtdPolicy MtdPolicy { get; set; }
    }
}