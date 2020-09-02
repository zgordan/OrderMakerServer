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
    public class ActivitesModel : PageModel
    {
        private readonly OrderMakerContext context;

        public ActivitesModel(OrderMakerContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }

        public async Task<IActionResult> OnGetAsync(string formId)
        {
            MtdForm = await context.MtdForm
                .Include(m => m.MtdFormHeader)
                .Include(m => m.MtdFormPart)
                .Where(x => x.Id == formId).FirstOrDefaultAsync();

            if (MtdForm == null)
            {
                return NotFound();
            }

            ViewData["ImgSrcForm"] = FormHeaderHandler.GetImageSrc(MtdForm.MtdFormHeader);

            return Page();
        }
    }
}
