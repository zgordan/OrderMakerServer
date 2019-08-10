/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Identity.Pages.Users
{
    public class RightsModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;


        public RightsModel(OrderMakerContext context, UserManager<WebAppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string UserName { get; set; }
        public string UserTitle { get; set; }
        public string UserId { get; set; }
        public IList<Claim> Claims { get; set; }
        public IList<MtdForm> MtdForms { get; set; }
        public IList<MtdGroup> MtdGroups { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            Claims = await _userManager.GetClaimsAsync(user);
            MtdGroups = await _context.MtdGroup.OrderBy(x=>x.Name).ToListAsync();

            UserId = user.Id;
            UserName = user.UserName;
            UserTitle = user.Title;

            MtdForms = await _context.MtdForm.Include(x=>x.MtdFormPart).ToListAsync();
            return Page();
        }
    }
}