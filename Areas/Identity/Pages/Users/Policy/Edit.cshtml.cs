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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Policy
{
    public class EditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public EditModel(OrderMakerContext context)
        {
            _context = context;
        }

        public MtdPolicy MtdPolicy { get; set; }
        public IList<MtdForm> MtdForms { get; set; }
        public IList<MtdGroup> MtdGroups { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) { return NotFound(); }

            MtdPolicy = await _context.MtdPolicy
                .Include(i => i.MtdPolicyForms)                
                .Include(i => i.MtdPolicyParts)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (MtdPolicy == null) { return NotFound(); }

            MtdGroups = await _context.MtdGroup.OrderBy(x=>x.Name).ToListAsync();
            MtdForms = await _context.MtdForm.Include(x => x.MtdFormPart).OrderBy(x=>x.Sequence).ToListAsync();

            return Page();
        }

    }
}