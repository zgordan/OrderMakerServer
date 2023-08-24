/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class PartEditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public PartEditModel(OrderMakerContext context)
        {
            _context = context;
        }

        public MtdForm MtdForm { get; set; }
        public MtdFormPart MtdFormPart { get; set; }
        public List<MTDSelectListItem> Styles { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {

            MtdFormPart = await _context.MtdFormPart.Include(m => m.MtdFormPartHeader).FirstOrDefaultAsync(x => x.Id == id);

            if (MtdFormPart == null)
            {
                return NotFound();
            }

            MtdForm = await _context.MtdForm.Include(m => m.MtdFormHeader).Where(x => x.Id == MtdFormPart.MtdForm).FirstOrDefaultAsync();
            IList<MtdSysStyle> styles = await _context.MtdSysStyle.ToListAsync();

            Styles = new List<MTDSelectListItem>();
            styles.ToList().ForEach((style) =>
            {
                Styles.Add(new MTDSelectListItem { Id = style.Id.ToString(), Value = style.Name, Localized = true });
            });

            return Page();
        }
    }
}