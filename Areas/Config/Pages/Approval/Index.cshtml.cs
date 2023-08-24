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

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Approval
{

    public class IndexModel : PageModel
    {
        private OrderMakerContext _context;

        public IndexModel(OrderMakerContext context)
        {
            _context = context;
        }

        public IList<MtdApproval> MtdApprovals { get; set; }
        public string SearchText { get; set; }
        public async Task<IActionResult> OnGetAsync(string searchText)
        {
            var query = _context.MtdApproval.Include(x => x.MtdFormNavigation).ThenInclude(x => x.MtdFormDesk).AsQueryable();

            if (searchText != null)
            {
                string normText = searchText.ToUpper();
                query = query.Where(x => x.Name.ToUpper().Contains(normText) ||
                                        x.Description.ToUpper().Contains(normText)
                                        );
                SearchText = searchText;
            }

            MtdApprovals = await query.ToListAsync();
            return Page();
        }
    }
}