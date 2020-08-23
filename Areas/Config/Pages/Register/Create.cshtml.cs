using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Register
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext context;
        private readonly IStringLocalizer<CreateModel> localizer;


        public CreateModel(OrderMakerContext context, IStringLocalizer<CreateModel> localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        [BindProperty]
        public MtdRegister MtdRegister { get; set; }

        public IActionResult OnGet()
        {
            MtdRegister = new MtdRegister
            {
                Id = Guid.NewGuid().ToString(),
            };


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            await context.MtdRegister.AddAsync(MtdRegister);
            await context.SaveChangesAsync();
            
            return RedirectToPage("./Edit", new { id = MtdRegister.Id });
        }

    }
}
