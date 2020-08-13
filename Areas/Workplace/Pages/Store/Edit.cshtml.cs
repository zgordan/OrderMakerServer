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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store
{
    public class EditModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public EditModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }


        public MtdForm MtdForm { get; set; }
        public MtdStore MtdStore { get; set; }
        public IList<MtdForm> RelatedForms { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MtdStore = await _context.MtdStore.FirstOrDefaultAsync(m => m.Id == id);

            if (MtdStore == null)
            {
                return NotFound();
            }

            var user = await _userHandler.GetUserAsync(HttpContext.User);            
            bool isEditor = await _userHandler.IsEditor(user,MtdStore.MtdForm,MtdStore.Id);
            
            if (!isEditor) {
                return Forbid();
            }

            WebAppUser webUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, MtdStore.Id);
            ApprovalStatus approvalStatus = await approvalHandler.GetStatusAsync(webUser);

            if (approvalStatus == ApprovalStatus.Rejected)
            {
                return Forbid();
            }

            MtdForm = await _context.MtdForm.FindAsync(MtdStore.MtdForm);

            RelatedForms = await _context.MtdFormRelated.Include(x => x.MtdChildForm)
                 .Where(x => x.ParentFormId == MtdForm.Id).Select(x => x.MtdChildForm)
                 .OrderBy(x => x.Sequence)
                 .ToListAsync();

            return Page();
        }




    }
}
