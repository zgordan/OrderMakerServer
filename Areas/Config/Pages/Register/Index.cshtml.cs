using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Register
{
    public class IndexModel : PageModel
    {
        private readonly OrderMakerContext context;

        public string SearchText { get; set; }
        public IList<MtdRegister> MtdRegisters { get; set; }

        public IndexModel(OrderMakerContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> OnGetAsync(string searchText)
        {
            var query = context.MtdRegister.AsQueryable();

            if (searchText != null)
            {
                string normText = searchText.ToUpper();
                query = query.Where(x => x.Name.ToUpper().Contains(normText) ||
                                        x.Description.ToUpper().Contains(normText));
                SearchText = searchText;
            }

            MtdRegisters = await query.OrderBy(x=>x.Name).ToListAsync();
            return Page();
        }
    }
}
