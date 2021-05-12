using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class ActEditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public ActEditModel(OrderMakerContext context)
        {
            _context = context;
        }

        public MtdForm MtdForm { get; set; }
        public MtdFormActivity MtdFormActivity { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {

            MtdFormActivity = await _context.MtdFormActivites.FirstOrDefaultAsync(x => x.Id == id);

            if (MtdFormActivity == null)
            {
                return NotFound();
            }

            MtdForm = await _context.MtdForm.Include(m => m.MtdFormHeader).Where(x => x.Id == MtdFormActivity.MtdFormId).FirstOrDefaultAsync();
            ViewData["ImgSrcForm"] = FormHeaderHandler.GetImageSrc(MtdForm.MtdFormHeader);
      
            return Page();
        }


        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            var activityId = requestForm["activityId"];
            var activity = new MtdFormActivity { Id = activityId };
            _context.MtdFormActivites.Remove(activity);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
    }
}
