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
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class PartsModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public PartsModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }
        [BindProperty]
        public string CurrentPartId { get; set; }
        public IList<MtdFormPartField> MtdFormPartFields { get; set; }

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

            CurrentPartId = partId ?? MtdForm.MtdFormPart.Select(x => x.Id).FirstOrDefault();

            //   IList<string> partIds = MtdForm.MtdFormPart.Select(x => x.Id).ToList();

            MtdFormPartFields = await _context.MtdFormPartField
                .Where(x => x.MtdFormPart == CurrentPartId).OrderBy(x => x.Sequence).ToListAsync();

            return Page();
        }
    }
}