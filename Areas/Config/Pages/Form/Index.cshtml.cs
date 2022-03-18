/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

*/


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{
    public class IndexModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public IndexModel(OrderMakerContext context)
        {
            _context = context;
        }

        public IList<MtdForm> MtdForm { get;set; }
        public string SearchText { get; set; }

        public async Task<IActionResult> OnGetAsync(string searchText)
        {
            var query = _context.MtdForm.Include(m=>m.MtdFormDesk).AsQueryable();

            if (searchText != null) {
                string normText = searchText.ToUpper();
                query = query.Where(x => x.Name.ToUpper().Contains(normText) || 
                                        x.Description.ToUpper().Contains(normText)
                                        );
                SearchText = searchText;
            }

            MtdForm = await query.OrderBy(x=>x.Sequence).ToListAsync();
            return Page();

        }
    }
}
