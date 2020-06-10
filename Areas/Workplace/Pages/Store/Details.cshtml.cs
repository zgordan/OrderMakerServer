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
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
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

        public List<MTDSelectListItem> ListResolutions { get; set; }
        public List<MTDSelectListItem> ListRejections { get; set; }

        public List<MTDSelectListItem> UsersList { get; set; }
        public List<MTDSelectListItem> Stages { get; set; }
        public List<MTDSelectListItem> UsersRequest { get; set; }

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
            
            if (!isViewer)
            {
                return NotFound();
            }

            IsEditor = await _userHandler.IsEditor(user, MtdStore.MtdForm, MtdStore.Id);
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

            
            UsersList = new List<MTDSelectListItem>();

            if (IsInstallerOwner)
            {
                List<WebAppUser> webAppUsers = new List<WebAppUser>();
                bool isViewAll = await _userHandler.GetFormPolicyAsync(user, MtdStore.MtdForm, RightsType.ViewAll);
                
                if (isViewAll)
                {
                    webAppUsers = await _userHandler.Users.ToListAsync();
                }
                else
                {
                    webAppUsers = await _userHandler.GetUsersInGroupsAsync(user);
                }

                
                webAppUsers.OrderBy(x => x.Title).ToList().ForEach((item) => {
                    UsersList.Add(new MTDSelectListItem { 
                         Id = item.Id, Value = item.Title
                    });
                });
             
            }

            UsersRequest = new List<MTDSelectListItem>();
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

                usersRequest.OrderBy(x => x.Title).ToList().ForEach((item) =>
                {
                    UsersRequest.Add(new MTDSelectListItem { Id = item.Id, Value = item.Title });
                });                
            }

            Stages = new List<MTDSelectListItem>();
            IList<MtdApprovalStage> stages = await approvalHandler.GetStagesDownAsync();
            stages.OrderBy(x=>x.Stage).ToList().ForEach((stage) => {
                Stages.Add( new MTDSelectListItem { Id=stage.Id.ToString(), Value = stage.Name });
            });
            
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
                    IsSign = log.IsSign != 0,
                    UserRecipient = log.UserRecipientName,              
                };

                ApprovalHistory.Add(temp);
            }

            await FilListResolutions(approvalHandler);

            return Page();
        }

        private async Task FilListResolutions(ApprovalHandler approvalHandler)
        {

            MtdApprovalStage currentStage = await approvalHandler.GetCurrentStageAsync();
            if (currentStage != null)
            {
                List<MtdApprovalRejection> listRejections = await _context.MtdApprovalRejection
                    .Where(x => x.MtdApprovalStageId == currentStage.Id).OrderBy(x => x.Sequence).ToListAsync();
                ListRejections = new List<MTDSelectListItem>();
                listRejections.ForEach((item) => {
                    ListRejections.Add(new MTDSelectListItem
                    {
                         Id = item.Id, Value = item.Name
                    });
                });

                List<MtdApprovalResolution> listResolution = await _context.MtdApprovalResolution
                    .Where(x => x.MtdApprovalStageId == currentStage.Id).OrderBy(x => x.Sequence).ToListAsync();
                ListResolutions = new List<MTDSelectListItem>();
                listResolution.ForEach((item) => {
                    ListResolutions.Add(new MTDSelectListItem
                    {
                        Id = item.Id,
                        Value = item.Name
                    });
                });
            }
        }


    }
}
