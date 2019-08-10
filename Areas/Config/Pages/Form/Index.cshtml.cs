/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Form
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

            MtdForm = await query.ToListAsync();
            return Page();

        }
    }
}
