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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Form
{
    public class PartEditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public PartEditModel(OrderMakerContext context)
        {
            _context = context;
        }

        public MtdForm MtdForm { get; set; }
        public MtdFormPart MtdFormPart { get; set; }       
        public async Task<IActionResult>  OnGetAsync(string id) {

            MtdFormPart = await _context.MtdFormPart.Include(m=>m.MtdFormPartHeader).FirstOrDefaultAsync(x=>x.Id == id);

            if (MtdFormPart == null)
            {
                return NotFound();
            }
            
            MtdForm = await _context.MtdForm.Include(m => m.MtdFormHeader).Where(x => x.Id == MtdFormPart.MtdForm).FirstOrDefaultAsync();
            IList<MtdSysStyle> styles =  await _context.MtdSysStyle.ToListAsync();
            ViewData["Styles"] = new SelectList(styles, "Id", "Name");

            return Page();
        }
    }
}