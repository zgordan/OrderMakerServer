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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Users
{
    [Authorize(Roles = "Admin")]
    public partial class UsersController : ControllerBase
    {

        [HttpPost("admin/confirm/email")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAdminConfimEmailAsync()
        {
            string userName = Request.Form["UserName"];
            WebAppUser user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            string userId = await _userManager.GetUserIdAsync(user);
            string email = await _userManager.GetEmailAsync(user);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
                protocol: Request.Scheme);

            BlankEmail blankEmail = new BlankEmail
            {
                Email = email,
                Subject = _localizer["Email Verification Procedure"],
                Header = _localizer["Email Verification Procedure"],
                Content = new List<string>()
                       {                           
                           _localizer["Confirm the ownership of the mailbox by clicking on the link below"],
                           $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["Email Verification"]}</a>"
                       }
            };

            await _emailSender.SendEmailBlankAsync(blankEmail);

            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpPost("admin/confirm/password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAdminConfirmPasswordAsync()
        {

            string userName = Request.Form["UserName"];
            WebAppUser user = await _userManager.FindByNameAsync(userName);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return NotFound();
            }

            string email = await _userManager.GetEmailAsync(user);
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            BlankEmail blankEmail = new BlankEmail
            {
                Email = email,
                Subject = _localizer["Password reset"],
                Header = _localizer["Password reset"],
                Content = new List<string>()
                       {
                           $"{_localizer["Your login"]}: <strong>{user.UserName}</strong>",
                           _localizer["To change your account password, follow the link below"],
                           $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["Create account password"]}</a>"
                       }
            };

            await _emailSender.SendEmailBlankAsync(blankEmail);

            return Ok();
        }

    }
}
