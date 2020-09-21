using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Config.Pages.Approval;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Extensions;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Store
{

    public partial class DataController : ControllerBase
    {

        [HttpPost("task/add")]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> OnPostTaskAddAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            string formId = requestForm["formId"];
            string storeId = requestForm["storeId"];
            string taskName = requestForm["taskName"];
            string taskDeadline = requestForm["taskDeadline"];
            string taskPrivate = requestForm["taskPrivate"];
            string taskInitNote = requestForm["taskInitNote"];
            string taskExecutor = requestForm["taskExecutor"];
            string dateFormat = requestForm["date-format"];

            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isViewer = await _userHandler.IsViewer(user, formId, storeId);
            if (!isViewer) { return NotFound(); }


            bool isDate = DateTime.TryParseExact(taskDeadline, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deadLine);
            if (!isDate) { deadLine = DateTime.Now; }

            MtdStoreTask mtdStoreTask = new MtdStoreTask()
            {
                Id = Guid.NewGuid().ToString(),
                MtdStoreId = storeId,
                Name = taskName,
                Deadline = deadLine,
                Initiator = user.Id,
                InitNote = taskInitNote,
                InitTimeCr = DateTime.Now,
                Executor = taskExecutor,
                ExecNote = string.Empty,
                ExecTimeCr = DateTime.Now,
                LastEventTime = DateTime.Now.AddHours(10),
                Complete = 0,
                PrivateTask = taskPrivate == "true" ? (sbyte)1 : (sbyte)0,
            };


            try
            {
                await _context.MtdStoreTasks.AddAsync(mtdStoreTask);
                await _context.SaveChangesAsync();

            }
            catch
            {
                return BadRequest(localizer["Error saving object."]);
            }


            try
            {
                if (mtdStoreTask.Initiator != mtdStoreTask.Executor)
                {
                    WebAppUser userExecutor = await _userHandler.FindByIdAsync(mtdStoreTask.Executor);
                    MtdStore mtdStore = await _context.MtdStore.FindAsync(storeId);
                    MtdForm mtdForm = await _context.MtdForm.FindAsync(formId);
                    string initName = user.GetFullName();
                    await SendEmailTaskAddAsync(initName, userExecutor, mtdStore, mtdForm, mtdStoreTask);
                }

            }
            catch
            {
                return BadRequest(localizer["Error sending message."]);
            }

            return Ok();
        }


        [HttpPost("task/close")]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> OnPostTaskCloseAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            string formId = requestForm["formId"];
            string storeId = requestForm["storeId"];
            string taskId = requestForm["taskId"];
            string resultValue = requestForm["resultValue"];
            string taskCloseComment = requestForm["taskCloseComment"];

            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);

            MtdStoreTask mtdStoreTask = await _context.MtdStoreTasks.FindAsync(taskId);
            if (mtdStoreTask == null) { return NotFound(); }

            bool isExecutor = mtdStoreTask.Executor == user.Id;
            if (!isExecutor || mtdStoreTask.Complete != 0) { return NotFound(); }

            bool isViewer = await _userHandler.IsViewer(user, formId, mtdStoreTask.MtdStoreId);
            if (!isViewer) { return NotFound(); }

            mtdStoreTask.ExecNote = taskCloseComment;
            mtdStoreTask.Complete = resultValue == "yes" ? 1 : -1;
            mtdStoreTask.ExecTimeCr = DateTime.Now;


            try
            {
                await _context.SaveChangesAsync();

            }
            catch
            {
                return BadRequest(localizer["Error saving object."]);
            }

            try
            {
                if (mtdStoreTask.Initiator != mtdStoreTask.Executor)
                {
                    WebAppUser userInitiator = await _userHandler.FindByIdAsync(mtdStoreTask.Initiator);
                    MtdStore mtdStore = await _context.MtdStore.FindAsync(storeId);
                    MtdForm mtdForm = await _context.MtdForm.FindAsync(formId);
                    string execName = user.GetFullName();
                    await SendEmailTaskCloseAsync(execName, userInitiator, mtdStore, mtdForm, mtdStoreTask);
                }
            }
            catch
            {
                return BadRequest(localizer["Error sending message."]);
            }


            return Ok();

        }


        [HttpPost("task/delete")]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> OnPostTaskDeleteAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            string taskId = requestForm["taskId"];

            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);

            MtdStoreTask storeTask = await _context.MtdStoreTasks.FindAsync(taskId);
            if (storeTask == null) { return NotFound(); }

            bool isInitiator = storeTask.Initiator == user.Id;
            if (!isInitiator) { return NotFound(); }

            _context.MtdStoreTasks.Remove(storeTask);
            await _context.SaveChangesAsync();


            return Ok();

        }


        private async Task<bool> SendEmailTaskAddAsync(string initName, WebAppUser userExecutor, MtdStore mtdStore, MtdForm mtdForm, MtdStoreTask mtdStoreTask)
        {

            BlankEmail blankEmail = new BlankEmail
            {
                Subject = localizer["Task management event"],
                Email = userExecutor.Email,
                Header = localizer["New Task"],
                Content = new List<string> {
                              $"<strong>{localizer["Document"]} - {mtdForm.Name}</strong>",
                              $"{localizer["User"]} {initName} {localizer["has created task for you at"]} {DateTime.Now}",
                              $"{localizer["Task:"]} {mtdStoreTask.Name}",
                              $"{localizer["Deadline:"]} {mtdStoreTask.Deadline}",
                              }
            };

            if (mtdStoreTask.InitNote.Length > 0)
            {
                blankEmail.Content.Add($"{localizer["User's comment"]}: <em>{mtdStoreTask.InitNote}</em>");
            }

            blankEmail.Content.Add($"{localizer["Click on the link to view the document."]}");
            blankEmail.Content.Add($"<a href='{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/workplace/store/details?id={mtdStore.Id}'>{localizer["Document link"]}</a>");
            await emailSender.SendEmailBlankAsync(blankEmail);

            return true;
        }


        private async Task<bool> SendEmailTaskCloseAsync(string execName, WebAppUser userInitiator, MtdStore mtdStore, MtdForm mtdForm, MtdStoreTask mtdStoreTask)
        {
            LocalizedString taskStatus = mtdStoreTask.Complete == 1 ? localizer["Task complete"] : localizer["Task refused"];
            BlankEmail blankEmail = new BlankEmail
            {
                Subject = localizer["Task management event"],
                Email = userInitiator.Email,
                Header = localizer["Task closed"],
                Content = new List<string> {
                              $"<strong>{localizer["Document"]} - {mtdForm.Name}</strong>",
                              $"{localizer["User"]} {execName} {localizer["has closed the task at"]} {DateTime.Now}",
                              $"{localizer["Task:"]} {mtdStoreTask.Name}",
                              $"{localizer["Deadline:"]} {mtdStoreTask.Deadline}",
                              $"{localizer["Status:"]} {taskStatus}",
                              }
            };

            if (mtdStoreTask.ExecNote.Length > 0)
            {
                blankEmail.Content.Add($"{localizer["User's comment"]}: <em>{mtdStoreTask.ExecNote}</em>");
            }

            blankEmail.Content.Add($"{localizer["Click on the link to view the document."]}");
            blankEmail.Content.Add($"<a href='{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/workplace/store/details?id={mtdStore.Id}'>{localizer["Document link"]}</a>");
            await emailSender.SendEmailBlankAsync(blankEmail);

            return true;
        }

    }
}
