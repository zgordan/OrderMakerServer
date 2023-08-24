/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class PartsModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public PartsModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }
        [BindProperty]
        public string CurrentPartId { get; set; }
        public IList<MtdFormPartField> MtdFormPartFields { get; set; }

        public async Task<IActionResult> OnGetAsync(string formId, string partId)
        {


            MtdForm = await _context.MtdForm
                .Include(m => m.MtdFormHeader)
                .Include(m => m.MtdFormPart)
                .Where(x => x.Id == formId).FirstOrDefaultAsync();

            if (MtdForm == null)
            {
                return NotFound();
            }

            CurrentPartId = partId ?? MtdForm.MtdFormPart.Select(x => x.Id).FirstOrDefault();

            //   IList<string> partIds = MtdForm.MtdFormPart.Select(x => x.Id).ToList();

            MtdFormPartFields = await _context.MtdFormPartField
                .Where(x => x.MtdFormPart == CurrentPartId).OrderBy(x => x.Sequence).ToListAsync();

            return Page();
        }
    }
}