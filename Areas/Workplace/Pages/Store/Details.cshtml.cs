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
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Approval;
using Mtd.OrderMaker.Web.Models.LogDocument;
using Mtd.OrderMaker.Web.Models.Store;
using Mtd.OrderMaker.Web.Services;

namespace Mtd.OrderMaker.Web.Areas.Workplace.Pages.Store
{
    public class DetailsModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public DetailsModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        public MtdStore MtdStore { get; set; }
        public MtdForm MtdForm { get; set; }
        public ChangesHistory ChangesHistory { get; set; }
        public MtdStoreOwner StoreOwner { get; set; }
        public bool IsInstallerOwner { get; set; }
        public bool IsEditor { get; set; }
        public bool IsApprover { get; set; }
        public bool IsFirstStage { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public List<MtdFormPart> BlockParts { get; set; }
        public bool IsFormApproval { get; set; }

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
            bool isViewer = await _userHandler.IsViewer(user, MtdStore.MtdForm, MtdStore.Id);

            IsEditor = await _userHandler.IsEditor(user, MtdStore.MtdForm, MtdStore.Id);
            IsInstallerOwner = await _userHandler.IsInstallerOwner(user, MtdStore.MtdForm);

            if (!isViewer)
            {
                return Forbid();
            }


            MtdForm = await _context.MtdForm.Include(m => m.InverseParentNavigation).FirstOrDefaultAsync(x => x.Id == MtdStore.MtdForm);
            MtdLogDocument edited = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderByDescending(x => x.TimeCh).FirstOrDefaultAsync();
            MtdLogDocument created = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderBy(x => x.TimeCh).FirstOrDefaultAsync();

            StoreOwner = await _context.MtdStoreOwner.Where(x => x.Id == MtdStore.Id).FirstOrDefaultAsync();
            ChangesHistory = new ChangesHistory();

            if (edited != null)
            {
                ChangesHistory.LastEditedUser = edited.UserName;
                ChangesHistory.LastEditedTime = edited.TimeCh.ToString();
            }

            if (created != null)
            {
                ChangesHistory.CreateByTime = created.TimeCh.ToString();
                ChangesHistory.CreateByUser = created.UserName;
            }


            if (IsInstallerOwner)
            {

                List<WebAppUser> webAppUsers = new List<WebAppUser>();
                bool isViewAll = await _userHandler.GetFormPolicyAsync(user, MtdStore.MtdForm, RightsType.View);
                if (isViewAll)
                {
                    webAppUsers = await _userHandler.Users.ToListAsync();
                }
                else
                {
                    webAppUsers = await _userHandler.GetUsersInGroupsAsync(user);
                }

                ViewData["UsersList"] = new SelectList(webAppUsers.OrderBy(x=>x.Title), "Id", "Title");
            }



            ApprovalHandler approvalHandler = new ApprovalHandler(_context, MtdStore.Id);
            IsApprover = await approvalHandler.IsApproverAsync(user);
            IsFirstStage = await approvalHandler.IsFirstStageAsync();
            IList<MtdApprovalStage> stages = await approvalHandler.GetStagesDownAsync();
            ViewData["Stages"] = new SelectList(stages.OrderByDescending(x => x.Stage), "Id", "Name");

            List<string> partIds = await approvalHandler.GetWilBeBlockedPartsIds();
            BlockParts = new List<MtdFormPart>();
            if (partIds.Count > 0)
            {
                BlockParts = await _context.MtdFormPart.Where(x => partIds.Contains(x.Id)).OrderBy(x => x.Sequence).ToListAsync();
            }
            IsFormApproval = await approvalHandler.IsApprovalFormAsync();

            if (IsFormApproval)
            {
                ApprovalStatus = await approvalHandler.GetStatusAsync(user);

            }


            return Page();
        }


    }
}
