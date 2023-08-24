using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class ActCreateModel : PageModel
    {
        private readonly OrderMakerContext context;

        public ActCreateModel(OrderMakerContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }
        [BindProperty]
        public MtdFormActivity MtdFormActivity { get; set; }

        public async Task<IActionResult> OnGetAsync(string formId)
        {
            MtdForm = await context.MtdForm.Include(m => m.MtdFormHeader).Where(x => x.Id == formId).FirstOrDefaultAsync();

            if (MtdForm == null)
            {
                return NotFound();
            }

            ViewData["ImgSrcForm"] = FormHeaderHandler.GetImageSrc(MtdForm.MtdFormHeader);

            MtdFormActivity = new MtdFormActivity
            {
                Id = Guid.NewGuid().ToString(),
                MtdFormId = formId,

            };

            return Page();
        }
    }
}
