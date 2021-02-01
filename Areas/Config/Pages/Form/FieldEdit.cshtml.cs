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
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class FieldEditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public FieldEditModel(OrderMakerContext context)
        {
            _context = context;
        }
        
        public MtdForm MtdForm { get; set; }

        public MtdFormPartField MtdFormPartField { get; set; }

        public string FieldTypeName { get; set; }
        public string NameFormSelector { get; set; }

        public List<MTDSelectListItem> PartItems { get; set; }
        public List<MTDSelectListItem> TriggerItems { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            
            MtdFormPartField = await _context.MtdFormPartField.Include(x=>x.MtdFormList).FirstOrDefaultAsync(x=>x.Id==id);

            if (MtdFormPartField == null) {
                return NotFound();
            }

            MtdFormPart selfPart = await _context.MtdFormPart.FindAsync(MtdFormPartField.MtdFormPart);
         
            MtdForm = await _context.MtdForm.Include(m => m.MtdFormHeader).FirstOrDefaultAsync(x => x.Id == selfPart.MtdForm);

            IList<MtdFormPart> parts = await _context.MtdFormPart.Where(x=>x.MtdForm == MtdForm.Id).OrderBy(x=>x.Sequence).ToListAsync();
            IList<MtdSysTrigger> triggers = await _context.MtdSysTrigger.OrderBy(x => x.Sequence).ToListAsync();

            FieldTypeName = await _context.MtdSysType.Where(x => x.Id == MtdFormPartField.MtdSysType).Select(x => x.Name).FirstOrDefaultAsync();

            if (MtdFormPartField.MtdSysType == 11) {
                string formId = MtdFormPartField.MtdFormList.MtdForm;
                MtdForm selfForm = await _context.MtdForm.FindAsync(formId);
                NameFormSelector = selfForm.Name;
            }

            PartItems = new List<MTDSelectListItem>();
            parts.ToList().ForEach((item) =>
            {
                bool selected = selfPart.Id == item.Id;
                PartItems.Add(new MTDSelectListItem { Id = item.Id, Value = item.Name, Selectded = selected });
            });

            //ViewData["Parts"] = new SelectList(parts, "Id", "Name", selfPart.Id);

            TriggerItems = new List<MTDSelectListItem>();
            triggers.ToList().ForEach((item) => {
                bool selected = MtdFormPartField.MtdSysTrigger == item.Id;
                TriggerItems.Add(new MTDSelectListItem { Id = item.Id, Value=item.Name, Selectded=selected });
            });

           ///ViewData["Triggers"] = new SelectList(triggers, "Id", "Name", MtdFormPartField.MtdSysTrigger);

            return Page();
        }
    }
}