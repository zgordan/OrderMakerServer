/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class PartCreateModel : PageModel
    {

        private readonly OrderMakerContext _context;

        public PartCreateModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }

        [BindProperty]
        public MtdFormPart MtdFormPart { get; set; }


        public async Task<IActionResult> OnGetAsync(string formId)
        {
            MtdForm = await _context.MtdForm.Include(m => m.MtdFormHeader).Where(x => x.Id == formId).FirstOrDefaultAsync();

            if (MtdForm == null)
            {
                return NotFound();
            }

            MtdFormPart = new MtdFormPart
            {
                Id = Guid.NewGuid().ToString(),
                MtdForm = formId,

            };

            return Page();
        }
    }
}