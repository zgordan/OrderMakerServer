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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Entity;


namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly IStringLocalizer<CreateModel> _localizer;

        public CreateModel(OrderMakerContext context, IStringLocalizer<CreateModel> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }
        public IList<MtdForm> MtdForms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            MtdForm = new MtdForm
            {
                Id = Guid.NewGuid().ToString(),
            };

            MtdForms = await _context.MtdForm.ToListAsync();
            ViewData["Forms"] = new SelectList(MtdForms.OrderBy(x => x.Sequence), "Id", "Name");

            return Page();
        }
                       

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var group = await _context.MtdCategoryForm.FirstOrDefaultAsync();

            MtdForm.MtdCategory = group.Id;
            MtdForm.Parent = MtdForm.Parent == "null" ? null : MtdForm.Parent;
            MtdForm.Active = 1;

            _context.MtdForm.Add(MtdForm);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = MtdForm.Id });
        }
    }
}