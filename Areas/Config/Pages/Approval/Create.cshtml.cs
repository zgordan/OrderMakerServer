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
using Mtd.OrderMaker.Server.Data;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Approval
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public CreateModel(OrderMakerContext context)
        {
            _context = context;

        }

        [BindProperty]
        public MtdApproval MtdApproval { get; set; }
        public IList<MtdForm> MtdForms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            MtdApproval = new MtdApproval
            {
                Id = Guid.NewGuid().ToString()
            };

            IList<string> formsIds = await _context.MtdApproval.Select(x => x.MtdForm).ToListAsync();
            MtdForms = await _context.MtdForm.Where(x => !formsIds.Contains(x.Id)).ToListAsync();
            ViewData["Forms"] = new SelectList(MtdForms.OrderBy(x => x.Sequence), "Id", "Name");


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.MtdApproval.Add(MtdApproval);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = MtdApproval.Id });
        }
    }
}