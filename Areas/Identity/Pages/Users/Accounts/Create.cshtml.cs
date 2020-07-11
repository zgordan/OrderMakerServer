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

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.AppConfig;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Services;
using System.Collections.Generic;
using System.Linq;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Accounts
{
    public partial class CreateModel : PageModel
    {
        private readonly UserHandler _userManager;
        private readonly IEmailSenderBlank _emailSender;
        private readonly ILogger<CreateModel> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IOptions<LimitSettings> limits;

        public CreateModel(
            UserHandler userManager,
            IEmailSenderBlank emailSender,
            ILogger<CreateModel> logger,
            IStringLocalizer<SharedResource> localizer,
            IOptions<LimitSettings> limits)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _localizer = localizer;
            this.limits = limits;
        }

        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            public string Id { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string Title { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Display(Name = "Send Email")]
            public bool SendEmail { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public void OnGet()
        {
            Input = new InputModel { Id = Guid.NewGuid().ToString() };            
        }

        
        public async Task<IActionResult> OnPostAsync()
        {

            int userQty = _userManager.Users.Count();
            int userLimit = limits.Value.Users;

            if (userQty >= userLimit) { return BadRequest(_localizer["Limit users!"]); }

            string pass = _userManager.GeneratePassword();
            
            var user = new WebAppUser { Id = Input.Id, Title = Input.Title, UserName = Input.UserName, Email = Input.Email, 
                EmailConfirmed = Input.SendEmail, 
                PhoneNumber = Input.PhoneNumber };

            IdentityResult result;
            result = await _userManager.CreateAsync(user, pass);


            await _userManager.AddToRoleAsync(user, "Guest");


            if (!result.Succeeded) {

                IdentityError text = result.Errors.FirstOrDefault() ?? new IdentityError();
                return BadRequest(_localizer[text.Description]);
            }

            return RedirectToPage("./Edit", new { id = Input.Id});

        }
    }
}