/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mtd.OrderMaker.Server.Entity;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Approval
{
    public class StageCreateModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public StageCreateModel(OrderMakerContext context)
        {
            _context = context;
        }

        public MtdApproval MtdApproval { get; set; }

        public async Task<IActionResult> OnGetAsync(string idApproval)
        {

            MtdApproval = await _context.MtdApproval.FindAsync(idApproval);
            if (MtdApproval == null)
            {
                return NotFound();
            }

            return Page();
        }

    }
}