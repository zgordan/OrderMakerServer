/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public IndexModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        public IList<MtdForm> Forms { get; set; }
        public string SearchText { get; set; }

        public async Task<IActionResult> OnGetAsync(string searchText)
        {
            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);

            List<string> formIds = await _userHandler.GetViewFormIdsAsync(user);
            List<MtdForm> forms = await _context.MtdForm.Where(x => formIds.Contains(x.Id)).ToListAsync();

            foreach (var form in forms)
            {
                bool isExists = await _context.MtdFilter.Where(x => x.MtdForm == form.Id && x.IdUser==user.Id).AnyAsync();
                if (!isExists)
                {
                    MtdFilter mtdFilter = new MtdFilter
                    {
                        MtdForm = form.Id,
                        IdUser = user.Id,
                        Page = 1,
                        PageSize = 10,
                        SearchText = "",
                        ShowDate = 1,
                        WaitList = 0,
                        SearchNumber = "",
                        ShowNumber = 1
                    };

                    await _context.MtdFilter.AddAsync(mtdFilter);
                    await _context.SaveChangesAsync();
                }
            }

            IQueryable<MtdForm> query = _context.MtdForm
                    .Include(x => x.MtdCategoryNavigation)
                    .Include(x => x.MtdFormHeader)
                    .Include(x => x.MtdFormDesk)
                    .Where(x => formIds.Contains(x.Id));

            if (searchText != null)
            {
                query = query.Where(x => x.Name.Contains(searchText));
                SearchText = searchText;
            }


            Forms = await query.ToListAsync();
            return Page();
        }

    }
}
