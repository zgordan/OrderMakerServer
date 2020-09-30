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
using Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store.Models;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Extensions;
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
        public WebAppUser UserOwner { get; set; }
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

        public List<ApprovalLog> ApprovalHistory { get; set; }

        public List<MTDSelectListItem> ListResolutions { get; set; }
        public List<MTDSelectListItem> ListRejections { get; set; }

        public List<MTDSelectListItem> UsersList { get; set; }
        public List<MTDSelectListItem> UsersTask { get; set; }

        public List<MTDSelectListItem> Stages { get; set; }
        public List<MTDSelectListItem> UsersRequest { get; set; }
        public bool RelatedDocs { get; set; }
        public List<MTDSelectListItem> RelatedForms { get; set; }
        public List<string> ChildIds { get; set; }

        public bool ViewActivites { get; set; }
        public List<MTDSelectListItem> Activites { get; set; }
        public List<ActivityLine> ActivityLines { get; set; }

        public List<StoreTask> StoreTasks { get; set; }

        public WebAppUser CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) { return NotFound(); }

            MtdStore = await _context.MtdStore.Include(x => x.MtdFormNavigation.MtdFormHeader).FirstOrDefaultAsync(m => m.Id == id);

            if (MtdStore == null) { return NotFound(); }

            var user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isViewer = await _userHandler.IsViewer(user, MtdStore.MtdForm, MtdStore.Id);

            if (!isViewer)
            {
                return NotFound();
            }

            CurrentUser = user;
            IsEditor = await _userHandler.IsEditor(user, MtdStore.MtdForm, MtdStore.Id);
            IsInstallerOwner = await _userHandler.IsInstallerOwner(user, MtdStore.MtdForm);
            IsEraser = await _userHandler.IsEraser(user, MtdStore.MtdForm, MtdStore.Id);

            MtdForm = await _context.MtdForm.Include(m => m.InverseParentNavigation).FirstOrDefaultAsync(x => x.Id == MtdStore.MtdForm);
            MtdLogDocument edited = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderByDescending(x => x.TimeCh).FirstOrDefaultAsync();
            MtdLogDocument created = await _context.MtdLogDocument.Where(x => x.MtdStore == MtdStore.Id).OrderBy(x => x.TimeCh).FirstOrDefaultAsync();

            IsReviewer = await _userHandler.IsReviewer(user, MtdForm.Id);

            UserOwner = new WebAppUser();
            StoreOwner = await _context.MtdStoreOwner.Where(x => x.Id == MtdStore.Id).FirstOrDefaultAsync();
            if (StoreOwner != null) { UserOwner = await _userHandler.FindByIdAsync(StoreOwner.UserId); }

            ChangesHistory = new ChangesHistory();

            if (edited != null)
            {
                WebAppUser userEditor = await _userHandler.FindByIdAsync(edited.UserId);
                ChangesHistory.LastEditedUser = userEditor == null ? edited.UserName : userEditor.GetFullName();
                ChangesHistory.LastEditedTime = edited.TimeCh.ToString();
            }

            if (created != null)
            {
                WebAppUser userCreator = await _userHandler.FindByIdAsync(created.UserId);
                ChangesHistory.CreateByUser = userCreator == null ? created.UserName : userCreator.GetFullName();
                ChangesHistory.CreateByTime = created.TimeCh.ToString();
            }


            ApprovalHandler approvalHandler = new ApprovalHandler(_context, MtdStore.Id);

            await SetUsersTask();

            await SetUsersList(user);

            await SetUsersRequest(user, approvalHandler);

            await SetApprovalInfo(id, user, approvalHandler);

            await SetResolutions(approvalHandler);

            await SetRelatedForms(user);

            await SetActivites();

            await SetTasks(user);

            return Page();
        }


        private async Task SetTasks(WebAppUser currentUser)
        {
            StoreTasks = new List<StoreTask>();
            List<MtdStoreTask> storeTasks = await _context.MtdStoreTasks.AsNoTracking().Where(x => x.MtdStoreId == MtdStore.Id).OrderBy(x => x.Deadline).ToListAsync();
            if (storeTasks != null)
            {
                foreach (var st in storeTasks)
                {
                    if (st.PrivateTask == 1 && currentUser.Id != st.Initiator && currentUser.Id != st.Executor) { continue; }

                    StoreTask storeTask = new StoreTask
                    {
                        Id = st.Id,
                        Name = st.Name,
                        PrivateTask = st.PrivateTask == 1,
                        Deadline = st.Deadline,
                        InitTimeCr = st.InitTimeCr,
                        InitNote = st.InitNote,
                        ExecTimeCr = st.ExecTimeCr,
                        ExecNote = st.ExecNote,
                        TaskComplete = st.Complete == 1,
                        TaskRejected = st.Complete == -1,
                        ButtonClose = currentUser.Id == st.Executor && st.Complete == 0,
                        ButtonDelete = currentUser.Id == st.Initiator,
                    };

                    WebAppUser userInitiator = await _userHandler.FindByIdAsync(st.Initiator);
                    storeTask.Initiator = userInitiator.GetFullName();

                    WebAppUser userExecutor = await _userHandler.FindByIdAsync(st.Executor);
                    storeTask.Executor = userExecutor.GetFullName();

                    StoreTasks.Add(storeTask);
                }
            };
        }


        private async Task SetApprovalInfo(string id, WebAppUser user, ApprovalHandler approvalHandler)
        {
            IsApprover = await approvalHandler.IsApproverAsync(user);
            IsFirstStage = await approvalHandler.IsFirstStageAsync();

            Stages = new List<MTDSelectListItem>();
            IList<MtdApprovalStage> stages = await approvalHandler.GetStagesDownAsync();
            stages.OrderBy(x => x.Stage).ToList().ForEach((stage) =>
            {
                Stages.Add(new MTDSelectListItem { Id = stage.Id.ToString(), Value = stage.Name });
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
            foreach (var log in logs)
            {
                WebAppUser appUser = await _userHandler.FindByIdAsync(log.UserId);
                ApprovalLog temp = new ApprovalLog
                {
                    Time = log.Timecr,
                    UserName = appUser == null ? log.UserName : appUser.GetFullName(),
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
        }

        private async Task SetRelatedForms(WebAppUser user)
        {
            RelatedForms = new List<MTDSelectListItem>();

            List<MtdForm> relatedForms = await _context.MtdFormRelated.Include(x => x.MtdChildForm)
                .Where(x => x.ParentFormId == MtdForm.Id).Select(x => x.MtdChildForm)
                .OrderBy(x => x.Sequence)
                .ToListAsync();

            RelatedDocs = relatedForms.Any();

            if (relatedForms != null)
            {

                string selecteFormId = null;

                foreach (var form in relatedForms)
                {
                    bool viever = await _userHandler.IsViewer(user, form.Id);
                    bool creator = await _userHandler.CheckUserPolicyAsync(user, form.Id, RightsType.RelatedCreate);

                    if (viever && creator)
                    {
                        if (selecteFormId == null) { selecteFormId = form.Id; }
                        RelatedForms.Add(new MTDSelectListItem { Id = form.Id, Value = form.Name, Selectded = form.Id == selecteFormId });
                    }
                }
            }

            ChildIds = await _context.MtdStore.Where(x => x.Parent == MtdStore.Id).Select(x => x.Id).ToListAsync();

            if (ChildIds == null) { ChildIds = new List<string>(); }
        }

        private async Task SetActivites()
        {
            Activites = new List<MTDSelectListItem>();
            ActivityLines = new List<ActivityLine>();
            IList<MtdFormActivity> activites = await _context.MtdFormActivites.Where(x => x.MtdFormId == MtdForm.Id).OrderBy(x => x.Sequence).ToListAsync();
            foreach (var activity in activites)
            {
                Activites.Add(new MTDSelectListItem { Id = activity.Id, Value = activity.Name, Selectded = activity.Sequence <= 1 });
            }

            ViewActivites = Activites.Count > 0;

            if (ViewActivites)
            {
                IList<MtdStoreActivity> storeActivities = await _context.MtdStoreActivites.Where(x => x.MtdStoreId == MtdStore.Id).OrderBy(x => x.TimeCr).ToListAsync();
                List<string> actIds = storeActivities.Select(x => x.MtdFormActivityId).ToList();
                IList<MtdFormActivity> formActivities = await _context.MtdFormActivites.Where(x => actIds.Contains(x.Id)).ToListAsync();

                foreach (var activity in storeActivities)
                {
                    string imgSrc = string.Empty;
                    var formActivitity = formActivities.Where(x => x.Id == activity.MtdFormActivityId).FirstOrDefault();
                    if (formActivitity.Image != null)
                    {
                        string base64 = Convert.ToBase64String(formActivitity.Image);
                        imgSrc = string.Format("data:{0};base64,{1}", formActivitity.ImageType, base64);
                    }

                    WebAppUser webAppUser = await _userHandler.FindByIdAsync(activity.UserId);
                    string userName = webAppUser.GetFullName();
                    ActivityLines.Add(new ActivityLine { Id = activity.Id, Name = formActivitity.Name, Comment = activity.Comment, ImgSrc = imgSrc, TimeCr = activity.TimeCr, User = userName, UserId = activity.UserId });
                }


            }
        }

        private async Task SetUsersRequest(WebAppUser user, ApprovalHandler approvalHandler)
        {
            UsersRequest = new List<MTDSelectListItem>();
            if (IsApprover && !IsFirstStage)
            {

                List<WebAppUser> usersRequest = await _userHandler.GetUsersForViewingForm(MtdStore.MtdForm, MtdStore.Id);

                MtdApprovalStage stage = await approvalHandler.GetCurrentStageAsync();
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
        }


        private async Task SetUsersTask()
        {
            UsersTask = new List<MTDSelectListItem>();

            List<WebAppUser> viewUsers = await _userHandler.GetUsersForViewingForm(MtdForm.Id, MtdStore.Id);

            viewUsers.OrderBy(x => x.Title).ToList().ForEach((item) =>
            {
                UsersTask.Add(new MTDSelectListItem
                {
                    Id = item.Id,
                    Value = item.GetFullName()
                });
            });
        }

        private async Task SetUsersList(WebAppUser user)
        {

            UsersList = new List<MTDSelectListItem>();
            List<WebAppUser> webAppUsers = new List<WebAppUser>();
            bool isViewAll = await _userHandler.CheckUserPolicyAsync(user, MtdStore.MtdForm, RightsType.ViewAll);

            if (isViewAll)
            {
                webAppUsers = await _userHandler.Users.ToListAsync();
            }
            else
            {
                webAppUsers = await _userHandler.GetUsersInGroupsAsync(user);
            }

            webAppUsers.OrderBy(x => x.Title).ToList().ForEach((item) =>
            {
                UsersList.Add(new MTDSelectListItem
                {
                    Id = item.Id,
                    Value = item.GetFullName()
                });
            });
        }

        private async Task SetResolutions(ApprovalHandler approvalHandler)
        {

            MtdApprovalStage currentStage = await approvalHandler.GetCurrentStageAsync();
            if (currentStage != null)
            {
                List<MtdApprovalRejection> listRejections = await _context.MtdApprovalRejection
                    .Where(x => x.MtdApprovalStageId == currentStage.Id).OrderBy(x => x.Sequence).ToListAsync();
                ListRejections = new List<MTDSelectListItem>();
                listRejections.ForEach((item) =>
                {
                    ListRejections.Add(new MTDSelectListItem
                    {
                        Id = item.Id,
                        Value = item.Name
                    });
                });

                List<MtdApprovalResolution> listResolution = await _context.MtdApprovalResolution
                    .Where(x => x.MtdApprovalStageId == currentStage.Id).OrderBy(x => x.Sequence).ToListAsync();
                ListResolutions = new List<MTDSelectListItem>();
                listResolution.ForEach((item) =>
                {
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
