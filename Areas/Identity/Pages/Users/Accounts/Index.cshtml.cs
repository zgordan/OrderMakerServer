using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly UserHandler _userHandler;
        private readonly RoleManager<WebAppRole> _roleManager;
        private readonly OrderMakerContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;


        public IndexModel(UserHandler userHandler, RoleManager<WebAppRole> roleManager, OrderMakerContext context, IStringLocalizer<SharedResource> localizer)
        {
            _userHandler = userHandler;
            _roleManager = roleManager;
            _context = context;
            _localizer = localizer;
           
        }

        public IList<WebAppPerson> Accounts { get; set; }
        public string SearchText { get; set; }
        public List<MTDSelectListItem> GroupItems { get; set; }
        public async Task<IActionResult> OnGetAsync(string searchText, string filterGroup)
        {
            var query = _userHandler.Users;
            string groupId = filterGroup ?? string.Empty;

            if (searchText != null)
            {
                string normText = searchText.ToUpper();
                query = query.Where(x => x.Title.ToUpper().Contains(normText) ||
                                        x.UserName.ToUpper().Contains(normText) ||
                                        x.Email.ToUpper().Contains(normText) ||
                                        x.TitleGroup.ToUpper().Contains(normText));
                SearchText = searchText;
            }

            

            if (filterGroup != null && groupId != "all")
            {
                
                if (groupId != "not")
                {
                    IList<WebAppUser> gusers = await _userHandler.GetUsersInGroupAsync(groupId);
                    if (gusers != null)
                    {
                        query = query.Where(x => gusers.Select(x => x.Id).Contains(x.Id));
                    }                    
                }

                if (groupId == "not")
                {
                    IList<WebAppUser> gusers = await _userHandler.GetUsersInGroupAsync();
                    if (gusers != null)
                    {
                        query = query.Where(x => gusers.Select(x => x.Id).Contains(x.Id));
                    }
                }


            }

            Accounts = new List<WebAppPerson>();
            IList<WebAppUser> users = await query.ToListAsync();
            foreach (WebAppUser user in users)
            {
                IList<string> roles = await _userHandler.GetRolesAsync(user);
                Accounts.Add(new WebAppPerson
                {
                    User = user,
                    Role = await _roleManager.FindByNameAsync(roles.Where(x => !x.ToUpper().Contains("CPQ-")).FirstOrDefault()),
                    MtdPolicy = await _userHandler.GetPolicyForUserAsync(user)
                });
            }

            GroupItems = new List<MTDSelectListItem>()
            {
                new MTDSelectListItem { Id="all", Value="All Groups", Localized=true, Selectded = (groupId=="all" || groupId == string.Empty)  },
                new MTDSelectListItem { Id="not", Value="No Group", Localized=true, Selectded = groupId=="not" }
            };

            IList<MtdGroup> groups = await _context.MtdGroup.OrderBy(x => x.Name).ToListAsync();

            foreach (MtdGroup group in groups)
            {
                GroupItems.Add(new MTDSelectListItem { Id = group.Id, Value = group.Name, Selectded = groupId == group.Id });
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