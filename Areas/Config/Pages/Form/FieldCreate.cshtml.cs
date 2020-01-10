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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Data;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class FieldCreateModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public FieldCreateModel(OrderMakerContext context)
        {
            _context = context;
        }


        public MtdForm MtdForm { get; set; }
        
        public MtdFormPart MtdFormPart { get; set; }
        
        public MtdFormPartField MtdFormPartField { get; set; }

        public async Task<IActionResult> OnGetAsync(string idPart)
        {

            MtdFormPart = await _context.MtdFormPart.FindAsync(idPart);

            if (MtdFormPart == null)
            {
                return NotFound();
            }

            MtdForm = await _context.MtdForm.Include(m => m.MtdFormHeader).FirstOrDefaultAsync(x=>x.Id == MtdFormPart.MtdForm);

            string fieldId =  Guid.NewGuid().ToString();

            MtdFormPartField = new MtdFormPartField
            {
                Id = fieldId,  
                MtdFormList = new MtdFormList { Id = fieldId}
            };

            ViewData["fieldTypes"] = new SelectList(await _context.MtdSysType.Where(x=>x.Active).OrderBy(x=>x.Id).ToListAsync(), "Id", "Name",1);
            ViewData["fieldForms"] = new SelectList(await _context.MtdForm.ToListAsync(), "Id", "Name");

            return Page();
        }
    }
}