/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly IStringLocalizer<CreateModel> _localizer;
        private readonly IOptions<LimitSettings> limits;

        public CreateModel(OrderMakerContext context, IStringLocalizer<CreateModel> localizer, IOptions<LimitSettings> limits)
        {
            _context = context;
            _localizer = localizer;
            this.limits = limits;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }
        public IList<MtdForm> MtdForms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            MtdForm = new MtdForm
            {
                Id = Guid.NewGuid().ToString(),
            };

            MtdForms = await _context.MtdForm.ToListAsync();
            ViewData["Forms"] = new SelectList(MtdForms.OrderBy(x => x.Sequence), "Id", "Name");

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int formQty = await _context.MtdForm.CountAsync();
            int formLimit = limits.Value.Forms;

            if (formQty >= formLimit) { return BadRequest(_localizer["Limit forms!"]); }

            var group = await _context.MtdCategoryForm.FirstOrDefaultAsync();

            MtdForm.MtdCategory = group.Id;
            MtdForm.Parent = MtdForm.Parent == "null" ? null : MtdForm.Parent;
            MtdForm.Active = 1;

            _context.MtdForm.Add(MtdForm);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = MtdForm.Id });
        }
    }
}