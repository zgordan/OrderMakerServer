using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Store
{
    public partial class DataController : ControllerBase
    {
       
        [HttpPost("activity/add")]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> OnPostActivityAddAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            string formId = requestForm["formId"];
            string storeId = requestForm["storeId"];
            string activityDate = requestForm["activityDate"];
            string activitySelect = requestForm["activitySelect"];
            string activityComment = requestForm["activityComment"];
            
            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isViewer = await _userHandler.IsViewer(user, formId, storeId);
            if (!isViewer) { return NotFound(); }

            MtdStoreActivity storeActivity = new MtdStoreActivity()
            {
                Id = Guid.NewGuid().ToString(),
                MtdStoreId = storeId,
                Comment = activityComment,
                MtdFormActivityId = activitySelect,
                UserId = user.Id,                
            };
            
            bool isOkDate = DateTime.TryParse(activityDate, out DateTime dateTime);
            if (isOkDate) { storeActivity.TimeCr = dateTime; }

            await _context.MtdStoreActivites.AddAsync(storeActivity);
            await _context.SaveChangesAsync();

            return Ok();

        }


        [HttpPost("activity/delete")]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> OnPostActivityDeleteAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            string activityId = requestForm["activityId"];

            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);

            MtdStoreActivity activity = await _context.MtdStoreActivites.FindAsync(activityId);
            if (activity == null || activity.UserId != user.Id) { return NotFound(); }

            _context.MtdStoreActivites.Remove(activity);
            await _context.SaveChangesAsync();


            return Ok();

        }
    }
}
