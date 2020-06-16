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
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class FieldsModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public FieldsModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }
        [BindProperty]
        public string CurrentPartId { get; set; }
        public IList<MtdFormPartField> MtdFormPartFields {get;set;}
        public List<MTDSelectListItem> PartItems { get; set; }

        public async Task<IActionResult> OnGetAsync(string formId, string partId)
        {
            
            MtdForm = await _context.MtdForm                
                .Include(m => m.MtdFormHeader)
                .Include(m => m.MtdFormPart)                
                .Where(x => x.Id == formId).FirstOrDefaultAsync();

            if (MtdForm == null)
            {
                return NotFound();
            }

            
            CurrentPartId = partId ?? MtdForm.MtdFormPart.OrderBy(x => x.Sequence).Select(x=>x.Id).FirstOrDefault();            

            MtdFormPartFields = await _context.MtdFormPartField                
                .Where(x => x.MtdFormPart == CurrentPartId).OrderBy(x=>x.Sequence).ToListAsync();

            PartItems = new List<MTDSelectListItem>();
            MtdForm.MtdFormPart.OrderBy(x=>x.Sequence).GroupBy(x=>x.Id).Select(x=> new { Id=x.Key, Name=x.Select(x=>x.Name).FirstOrDefault() }).ToList().ForEach((item)=> {
                PartItems.Add(new MTDSelectListItem { Id=item.Id, Value=item.Name});
            });

            return Page();
        }
    }
}