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
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store.Models;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.DataHandler.Approval;
using Mtd.OrderMaker.Server.Models.LogDocument;
using Mtd.OrderMaker.Server.Models.Store;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store
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
        public bool IsEraser { get; set; }
        public bool IsApprover { get; set; }
        public bool IsReviewer { get; set; }        
        public bool IsFirstStage { get; set; }
        public bool IsSign { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public List<MtdFormPart> BlockParts { get; set; }
        public bool IsFormApproval { get; set; }
        public MtdApproval MtdApproval { get; set; }

        public List<ApprovalLog> ApprovalHistory  { get; set; }

        public List<MtdApprovalResolution> ListResolutions { get; set; }
        public List<MtdApprovalRejection> ListRejections { get; set; }

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
            
            if (!isViewer)
            {
                return Forbid();
            }

            IsInstallerOwner = await _userHandler.IsInstallerOwner(user, MtdStore.MtdForm);
            IsEraser = await _userHandler.IsEraser(user, MtdStore.MtdForm, MtdStore.Id);
            
            MtdForm = await _context.MtdForm.Include(m => m.InverseParentNavigation).FirstOrDefaultAsync(x => x.Id == MtdStore.MtdForm);
            MtdLogDocument edited = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderByDescending(x => x.TimeCh).FirstOrDefaultAsync();
            MtdLogDocument created = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderBy(x => x.TimeCh).FirstOrDefaultAsync();

            IsReviewer = await _userHandler.IsReviewer(user, MtdForm.Id);

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

            ApprovalHandler approvalHandler = new ApprovalHandler(_context, MtdStore.Id);
            IsApprover = await approvalHandler.IsApproverAsync(user);
            IsFirstStage = await approvalHandler.IsFirstStageAsync();

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

            if (IsApprover && !IsFirstStage)
            {
                List<WebAppUser> usersRequest = await _userHandler.GetUsersForViewingForm(MtdStore.MtdForm,MtdStore.Id);
                MtdApprovalStage stage =  await approvalHandler.GetCurrentStageAsync();
                List<string> userIds = await approvalHandler.GetUsersWaitSignAsync();
                IList<MtdApprovalStage> mas = await approvalHandler.GetStagesAsync();
                List<string> userInStagesIds = mas.Where(x => x.UserId != "owner").GroupBy(x => x.UserId).Select(x => x.Key).ToList();

                usersRequest = usersRequest.Where(x => !userIds.Contains(x.Id) 
                    && !userInStagesIds.Contains(x.Id) 
                    && x.Id != user.Id 
                    && x.Id != stage.UserId).ToList();
                
                ViewData["UsersRequest"] = new SelectList(usersRequest.OrderBy(x => x.Title), "Id", "Title");

            }
                                              
            IList<MtdApprovalStage> stages = await approvalHandler.GetStagesDownAsync();
            ViewData["Stages"] = new SelectList(stages.OrderByDescending(x => x.Stage), "Id", "Name");
            MtdApproval = await approvalHandler.GetApproval();
            List<string> partIds = await approvalHandler.GetWilBeBlockedPartsIds();
            BlockParts = new List<MtdFormPart>();
            if (partIds.Count > 0)
            {
                BlockParts = await _context.MtdFormPart.Where(x => partIds.Contains(x.Id) && x.Title == 1).OrderBy(x => x.Sequence).ToListAsync();
            }
            IsFormApproval = await approvalHandler.IsApprovalFormAsync();

            if (IsFormApproval)
            {
                ApprovalStatus = await approvalHandler.GetStatusAsync(user);

            }

            IsSign = await approvalHandler.IsSignAsync();

            IList<MtdLogApproval> logs = await _context.MtdLogApproval
                .Where(x => x.MtdStore == id).ToListAsync();

            bool isComplete = await approvalHandler.IsComplete();

            ApprovalHistory = new List<ApprovalLog>();
            foreach(var log in logs)
            {
                WebAppUser appUser = await _userHandler.FindByIdAsync(log.UserId);
                ApprovalLog temp = new ApprovalLog
                {
                    Time = log.Timecr,
                    UserName = appUser == null ? log.UserName : appUser.Title,
                    Result = log.Result,
                    ImgData = log.ImgData,
                    ImgType = log.ImgType,
                    Note = log.Note,
                    Color = log.Color,
                    Comment = log.Comment ?? string.Empty,
                    IsSign = log.IsSign == 0 ? false : true,
                    UserRecipient = log.UserRecipientName,              
                };

                ApprovalHistory.Add(temp);
            }

            MtdApprovalStage currentStage = await approvalHandler.GetCurrentStageAsync();
            if (currentStage != null)
            {
                ListRejections = await _context.MtdApprovalRejection
                    .Where(x => x.MtdApprovalStageId == currentStage.Id).OrderBy(x => x.Sequence).ToListAsync();
                ViewData["ListRejection"] = new SelectList(ListRejections, "Id", "Name");

                ListResolutions = await _context.MtdApprovalResolution
                    .Where(x => x.MtdApprovalStageId == currentStage.Id).OrderBy(x => x.Sequence).ToListAsync();
                ViewData["ListResolution"] = new SelectList(ListResolutions, "Id", "Name");
            }


            return Page();
        }


    }
}
