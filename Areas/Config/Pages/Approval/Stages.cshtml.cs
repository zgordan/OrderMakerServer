/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Approval
{
    public class StagesModel : PageModel
    {

        private readonly OrderMakerContext _context;

        public StagesModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdApproval MtdApproval { get; set; }                
        public IList<MtdApprovalStage> Stages { get; set; }
        public async Task<IActionResult> OnGetAsync(string idApproval)
        {
            MtdApproval = await _context.MtdApproval.Include(m => m.MtdApprovalStage)
                .Where(x => x.Id == idApproval).FirstOrDefaultAsync();

            if (MtdApproval == null)
            {
                return NotFound();
            }

            Stages = await _context.MtdApprovalStage.Where(x => x.MtdApproval == MtdApproval.Id).OrderBy(x => x.Stage).ToListAsync();

            return Page();
        }
    }
}