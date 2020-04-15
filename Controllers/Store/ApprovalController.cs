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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.DataHandler.Approval;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server;

namespace Mtd.OrderMaker.Server.Controllers.Store
{

    [Route("api/store/approval")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class ApprovalController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;
        private readonly IEmailSenderBlank _emailSender;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ApprovalController(OrderMakerContext context, UserHandler userHandler, IEmailSenderBlank emailSender, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _context = context;
            _userHandler = userHandler;
            _emailSender = emailSender;
            _localizer = sharedLocalizer;
        }

        [HttpPost("start")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStartAsync()
        {
            string storeId = Request.Form["id-store"];
            string resolutionId = Request.Form["id-resolution"];
            string comment = Request.Form["comment-start-id"];

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, storeId);
            bool isApprover = await approvalHandler.IsApproverAsync(webAppUser);
            if (!isApprover) { return NotFound(); }

            bool isOk = await approvalHandler.ActionApprove(webAppUser, resolutionId, comment);
            if (isOk)
            {
                await SendEmailStart(approvalHandler);
            }
            return Ok();
        }

        [HttpPost("approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostApproveAsync()
        {
            string storeId = Request.Form["id-store"];
            string resolutionId = Request.Form["id-resolution"];
            string comment = Request.Form["comment-confirm-id"];

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, storeId);
            bool isApprover = await approvalHandler.IsApproverAsync(webAppUser);
            if (!isApprover) { return NotFound(); }

            bool isOk = await approvalHandler.ActionApprove(webAppUser, resolutionId, comment);
            if (isOk)
            {
                await SendEmailApprove(approvalHandler);
            }
            return Ok();
        }

        [HttpPost("reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostRejectAsync()
        {
            string storeId = Request.Form["id-store"];
            string rejectionId = Request.Form["id-rejection"];
            string comment = Request.Form["comment-reject-id"];

            bool completeOk = bool.TryParse(Request.Form["checkbox-complete"], out bool completeCheck);
            bool stageOk = int.TryParse(Request.Form["next-stage"], out int stageId);
            if (!stageOk || !completeOk) { return NotFound(); }

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, storeId);

            bool isFirstStage = await approvalHandler.IsFirstStageAsync();
            bool isApprover = await approvalHandler.IsApproverAsync(webAppUser);
            if (!isApprover || isFirstStage) { return NotFound(); }

            bool isOk = await approvalHandler.ActionReject(completeCheck, stageId, webAppUser, rejectionId, comment);
            if (isOk)
            {
                await SendEmailReject(approvalHandler);
            }
            return Ok();
        }


        [HttpPost("restart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostNewAsync()
        {
            string storeId = Request.Form["id-store"];
            string formId = Request.Form["id-form"];

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, storeId);
            bool isReviewer = await _userHandler.IsReviewer(webAppUser, formId);

            if (!isReviewer) { return NotFound(); }

            bool isOk = await approvalHandler.ActionApproveReset(webAppUser);
            if (isOk)
            {
                await SendEmailReStart(approvalHandler);
            }
            return Ok();
        }


        private async Task<bool> SendEmailStart(ApprovalHandler approvalHandler)
        {

            string ownerId = await approvalHandler.GetOwnerID();
            WebAppUser userCurrent = await _userHandler.GetUserAsync(HttpContext.User);
            WebAppUser userOwner = _userHandler.Users.Where(x => x.Id == ownerId).FirstOrDefault();
            string storeId = await approvalHandler.GetStoreID();
            MtdForm mtdForm = await approvalHandler.GetFormAsync();

            MtdApprovalStage stageNext = await approvalHandler.GetNextStageAsync();

            if (await approvalHandler.IsFirstStageAsync())
            {
                WebAppUser userNext = _userHandler.Users.Where(x => x.Id == stageNext.UserId).FirstOrDefault();
                BlankEmail blankEmail = new BlankEmail
                {
                    Subject = _localizer["Approval event"],
                    Email = userNext.Email,
                    Header = _localizer["Approval required"],
                    Content = new List<string> {
                        $"<strong>{_localizer["Document"]} - {mtdForm.Name}</strong>",
                        $"{_localizer["User"]} {userCurrent.Title} {_localizer["started a new approval at"]} {DateTime.Now}",
                        $"{_localizer["Click on the link to view the document that required to approve."]}",
                        $"<a href='http://{HttpContext.Request.Host}/workplace/store/details?id={storeId}'>{_localizer["Document link"]}</a>"}
                };

                await _emailSender.SendEmailBlankAsync(blankEmail);
            }

            return true;
        }

        private async Task<bool> SendEmailReStart(ApprovalHandler approvalHandler)
        {

            string ownerId = await approvalHandler.GetOwnerID();
            WebAppUser userCurrent = await _userHandler.GetUserAsync(HttpContext.User);
            WebAppUser userOwner = _userHandler.Users.Where(x => x.Id == ownerId).FirstOrDefault();
            string storeId = await approvalHandler.GetStoreID();
            MtdForm mtdForm = await approvalHandler.GetFormAsync();

            MtdApprovalStage stageNext = await approvalHandler.GetNextStageAsync();

            BlankEmail blankEmail = new BlankEmail
            {
                Subject = _localizer["Approval event"],
                Email = userOwner.Email,
                Header = _localizer["Approval process event"],
                Content = new List<string> {
                              $"<strong>{_localizer["Document"]} - {mtdForm.Name}</strong>",
                              $"{_localizer["User"]} {userCurrent.Title} {_localizer["restarted approval workflow at"]} {DateTime.Now}",
                              $"{_localizer["Click on the link to view the document and start new approval."]}",
                              $"<a href='http://{HttpContext.Request.Host}/workplace/store/details?id={storeId}'>{_localizer["Document link"]}</a>"}
            };
            await _emailSender.SendEmailBlankAsync(blankEmail);

            return true;
        }

        private async Task<bool> SendEmailApprove(ApprovalHandler approvalHandler)
        {

            string ownerId = await approvalHandler.GetOwnerID();
            WebAppUser userCurrent = await _userHandler.GetUserAsync(HttpContext.User);
            WebAppUser userOwner = _userHandler.Users.Where(x => x.Id == ownerId).FirstOrDefault();
            string storeId = await approvalHandler.GetStoreID();
            MtdForm mtdForm = await approvalHandler.GetFormAsync();

            MtdApprovalStage stageNext = await approvalHandler.GetNextStageAsync();

            if (stageNext != null)
            {
                WebAppUser userNext = _userHandler.Users.Where(x => x.Id == stageNext.UserId).FirstOrDefault();
                BlankEmail blankEmail = new BlankEmail
                {
                    Subject = _localizer["Approval event"],
                    Email = userNext.Email,
                    Header = _localizer["Approval required"],
                    Content = new List<string> {
                        $"<strong>{_localizer["Document"]} - {mtdForm.Name}</strong>",
                        $"{_localizer["User"]} {userCurrent.Title} {_localizer["approved the document at"]} {DateTime.Now}",
                        $"{_localizer["Click on the link to view the document that required to approve."]}",
                        $"<a href='http://{HttpContext.Request.Host}/workplace/store/details?id={storeId}'>{_localizer["Document link"]}</a>"}
                };

                await _emailSender.SendEmailBlankAsync(blankEmail);
            }
            else
            {
                BlankEmail blankEmail = new BlankEmail
                {
                    Subject = _localizer["Approval event"],
                    Email = userOwner.Email,
                    Header = _localizer["Approval process event"],
                    Content = new List<string> {
                              $"<strong>{_localizer["Document"]} - {mtdForm.Name}</strong>",
                              $"{_localizer["User"]} {userCurrent.Title} {_localizer["approved the document at"]} {DateTime.Now}",
                              $"{_localizer["Approval process is complete. Click on the link to view the document."]}",
                              $"<a href='http://{HttpContext.Request.Host}/workplace/store/details?id={storeId}'>{_localizer["Document link"]}</a>"}
                };
                await _emailSender.SendEmailBlankAsync(blankEmail);
            }

            return true;
        }

        private async Task<bool> SendEmailReject(ApprovalHandler approvalHandler)
        {
            string ownerId = await approvalHandler.GetOwnerID();
            WebAppUser userCurrent = await _userHandler.GetUserAsync(HttpContext.User);
            WebAppUser userOwner = _userHandler.Users.Where(x => x.Id == ownerId).FirstOrDefault();
            MtdForm mtdForm = await approvalHandler.GetFormAsync();
            string storeId = await approvalHandler.GetStoreID();

            MtdApprovalStage stagePrev = await approvalHandler.GetPrevStage();
            MtdApprovalStage stageFirst = await approvalHandler.GetFirstStageAsync();

            bool cacheReload = false;

            if (stagePrev != null)
            {
                WebAppUser user = userOwner;
                if (stagePrev.UserId != "owner")
                {
                    user = _userHandler.Users.Where(x => x.Id == stagePrev.UserId).FirstOrDefault();
                }

                BlankEmail blankEmail = new BlankEmail
                {
                    Subject = _localizer["Approval event"],
                    Email = user.Email,
                    Header = _localizer["Approval required"],
                    Content = new List<string> {
                        $"<strong>{_localizer["Document"]} - {mtdForm.Name}</strong>",
                        $"{_localizer["User"]} {userCurrent.Title} {_localizer["rejected the document at"]} {DateTime.Now}",
                        $"{_localizer["Click on the link to view the document that required to approve."]}",
                        $"<a href='http://{HttpContext.Request.Host}/workplace/store/details?id={storeId}'>{_localizer["Document link"]}</a>"}
                };


                approvalHandler.ClearCache();
                cacheReload = true;

                if (!await approvalHandler.IsComplete())
                {
                    await _emailSender.SendEmailBlankAsync(blankEmail);
                }

            }

            if (!cacheReload)
            {
                approvalHandler.ClearCache();
            }

            if (await approvalHandler.IsComplete())
            {
                BlankEmail blankOwner = new BlankEmail
                {
                    Subject = _localizer["Approval event"],
                    Email = userOwner.Email,
                    Header = _localizer["Approval process event"],
                    Content = new List<string> {
                        $"<strong>{_localizer["Document"]} - {mtdForm.Name}</strong>",
                        $"{_localizer["User"]} {userCurrent.Title} {_localizer["rejected the document at"]} {DateTime.Now}",
                        $"{_localizer["Approval process is complete. Click on the link to view the document."]}",
                        $"<a href='http://{HttpContext.Request.Host}/workplace/store/details?id={storeId}'>{_localizer["Document link"]}</a>"}
                };

                await _emailSender.SendEmailBlankAsync(blankOwner);
            }

            return true;
        }

    }
}
