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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.DataConfig;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Controllers.Users
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public partial class UsersController : ControllerBase
    {
        private readonly UserManager<WebAppUser> _userManager;
        private readonly RoleManager<WebAppRole> _roleManager;     
        private readonly IEmailSenderBlank _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly OrderMakerContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IOptions<ConfigSettings> _options;


        public UsersController(
            UserManager<WebAppUser> userManager,
            RoleManager<WebAppRole> roleManager,   
            IEmailSenderBlank emailSender,
            IWebHostEnvironment hostingEnvironment,
            OrderMakerContext context,
            IStringLocalizer<SharedResource> localizer,
            IOptions<ConfigSettings> options
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;   
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _localizer = localizer;
            _options = options;
        }


        [HttpPost("admin/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAdminDeleteAsync()
        {
            string userId = Request.Form["user-delete-id"];
            WebAppUser user = await _userManager.FindByIdAsync(userId);

            IList<MtdFilter> mtdFilters = await _context.MtdFilter.Where(x => x.IdUser == user.Id).ToListAsync();
            _context.MtdFilter.RemoveRange(mtdFilters);
            await _context.SaveChangesAsync();
            await _userManager.DeleteAsync(user);
            return Ok();
        }

        [HttpPost("admin/profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAdminProfileAsync()
        {

            string username = Request.Form["UserName"];

            if (username == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            string email = Request.Form["Input.Email"];
            string title = Request.Form["Input.Title"];
            string titleGroup = Request.Form["Input.TitleGroup"];
            string phone = Request.Form["Input.PhoneNumber"];
            string roleId = Request.Form["Input.Role"];
            string policyId = Request.Form["Input.Policy"];

            WebAppRole roleUser = await _roleManager.FindByIdAsync(roleId);

            string[] formConfirm = Request.Form["Input.IsConfirm"];
            bool isConfirm = false;
            if (formConfirm.FirstOrDefault() != null)
            {
                isConfirm = bool.Parse(formConfirm.FirstOrDefault());
            }

            if (user.Email != email)
            {
                user.Email = email;
                user.EmailConfirmed = false;
            }

            if (user.Title != title)
            {
                user.Title = title;
            }

            if (user.TitleGroup != titleGroup)
            {
                user.TitleGroup = titleGroup;
            }

            if (user.PhoneNumber != phone)
            {
                user.PhoneNumber = phone;
            }

            if (isConfirm)
            {
                user.EmailConfirmed = true;
            }

            await _userManager.UpdateAsync(user);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, roleUser.Name);
    
            IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user);
            await _userManager.RemoveClaimsAsync(user, claims);

            Claim claim = new Claim("policy", policyId);
            await _userManager.AddClaimAsync(user, claim);

            IList<MtdGroup> groups = await _context.MtdGroup.ToListAsync();
            foreach (var group in groups)
            {
                string value = Request.Form[$"{group.Id}-group"];
                if (value == "true")
                {
                    Claim claimGroup = new Claim("group", group.Id);
                    await _userManager.AddClaimAsync(user, claimGroup);
                }

            }

            return Ok();
        }

    }
}