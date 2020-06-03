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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class PartCreateModel : PageModel
    {

        private readonly OrderMakerContext _context;

        public PartCreateModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }

        [BindProperty]
        public MtdFormPart MtdFormPart { get; set; }


        public async Task<IActionResult> OnGetAsync(string formId)
        {
            MtdForm = await _context.MtdForm.Include(m => m.MtdFormHeader).Where(x => x.Id == formId).FirstOrDefaultAsync();

            if (MtdForm == null)
            {
                return NotFound();
            }

            MtdFormPart = new MtdFormPart
            {
                Id = Guid.NewGuid().ToString(),
                MtdForm = formId,

            };

            return Page();
        }
    }
}