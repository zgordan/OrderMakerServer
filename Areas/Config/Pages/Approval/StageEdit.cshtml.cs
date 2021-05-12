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
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Approval
{
    public class StageEditModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public StageEditModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }
        
        public MtdApproval MtdApproval { get; set; }
        public MtdApprovalStage MtdApprovalStage { get; set; }
        public IList<MtdFormPart> MtdFormParts { get; set; }
        public IList<MtdApprovalResolution> Resolutions { get; set; }
        public IList<MtdApprovalRejection> Rejections { get; set; }

        public List<MTDSelectListItem> UserItems { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {

            MtdApprovalStage = await _context.MtdApprovalStage.FindAsync(id);

            if (MtdApprovalStage == null)
            {
                return NotFound();
            }

            MtdApproval = await _context.MtdApproval.Include(x => x.MtdFormNavigation).Where(x => x.Id == MtdApprovalStage.MtdApproval).FirstOrDefaultAsync();
                        
            MtdFormParts = await _context.MtdFormPart.Where(x => x.MtdForm == MtdApproval.MtdForm).OrderBy(x => x.Sequence).ToListAsync();
            IList<WebAppUser> webAppUsers = await _userHandler.Users.OrderBy(x=>x.Title).ToListAsync();
            UserItems = new List<MTDSelectListItem>
            {
                new MTDSelectListItem { Id = "owner", Value = "Owner", Selectded = MtdApprovalStage.UserId == "owner", Localized=true }
            };
            webAppUsers.ToList().ForEach((item) =>
            {
                UserItems.Add(new MTDSelectListItem { Id = item.Id, Value = item.Title, Selectded = MtdApprovalStage.UserId == item.Id });
            });


            ViewData["Users"] = new SelectList(webAppUsers, "Id", "Title");

            Resolutions = await _context.MtdApprovalResolution.Where(x=>x.MtdApprovalStageId == id).OrderBy(x => x.Sequence).ToListAsync();
            Rejections = await _context.MtdApprovalRejection.Where(x => x.MtdApprovalStageId == id).OrderBy(x => x.Sequence).ToListAsync();

            return Page();
        }
    }
}