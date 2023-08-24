/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mtd.OrderMaker.Server.Entity;
using System;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Policy
{
    public class CreateModel : PageModel
    {

        public CreateModel() { }

        [BindProperty]
        public MtdPolicy MtdPolicy { get; set; }

        public void OnGet()
        {
            MtdPolicy = new MtdPolicy()
            {
                Id = Guid.NewGuid().ToString()
            };
        }



    }
}