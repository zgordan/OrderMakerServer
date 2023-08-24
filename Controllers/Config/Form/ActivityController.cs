using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Components;
using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Config.Form
{
    public partial class DataController : ControllerBase
    {
        [HttpPost("activity/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostActivityCreateAsync()
        {
            var requestForm = await Request.ReadFormAsync();

            string formId = requestForm["formId"];
            string activityId = requestForm["activityId"];
            string activityName = requestForm["activityName"];
            string activityNote = requestForm["activityNote"];

            int seq = await _context.MtdFormActivites.Where(x => x.MtdFormId == formId).MaxAsync(x => (int?)x.Sequence) ?? 0;

            MtdFormActivity activity = new MtdFormActivity
            {
                Id = activityId,
                MtdFormId = formId,
                Name = activityName,
                Description = activityNote,
                Sequence = ++seq
            };



            await _context.MtdFormActivites.AddAsync(activity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("activity/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostActivityEditAsync()
        {
            var requestForm = await Request.ReadFormAsync();

            string formId = requestForm["formId"];
            string activityId = requestForm["activityId"];
            string activityName = requestForm["activityName"];
            string activityNote = requestForm["activityNote"];


            MtdFormActivity activity = await _context.MtdFormActivites.FindAsync(activityId);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Attach(activity).State = EntityState.Modified;
            _context.Entry(activity).Property(x => x.MtdFormId).IsModified = false;

            activity.Name = activityName;
            activity.Description = activityNote;

            MTDImgSModify imgActivity = await MTDImgSelector.ImageModifyAsync("imgActivity", Request);
            activity.Image = imgActivity.Image;
            activity.ImageType = imgActivity.ImgType;
            _context.Entry(activity).Property(x => x.Image).IsModified = imgActivity.Modify;
            _context.Entry(activity).Property(x => x.ImageType).IsModified = imgActivity.Modify;

            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("activity/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostActivityDeleteAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            string activityId = requestForm["activityId"];



            MtdFormActivity activity = await _context.MtdFormActivites.FindAsync(activityId);
            if (activity == null)
            {
                return NotFound();
            }


            _context.MtdFormActivites.Remove(activity);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("activity/sequence")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostActivitySequenceAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            string strData = requestForm["fieldSeqData"];
            string formId = requestForm["formId"];

            string[] data = strData.Split("&");

            IList<MtdFormActivity> activites = await _context.MtdFormActivites.Where(x => x.MtdFormId == formId).ToListAsync();

            int counter = 0;
            foreach (string id in data)
            {
                var field = activites.Where(x => x.Id == id).FirstOrDefault();
                if (field != null)
                {
                    field.Sequence = counter;
                    counter++;
                }
            }

            _context.MtdFormActivites.UpdateRange(activites);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
