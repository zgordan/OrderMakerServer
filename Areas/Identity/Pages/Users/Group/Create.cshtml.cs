/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mtd.OrderMaker.Server.Entity;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Group
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public CreateModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdGroup MtdGroup { get; set; }

        public void OnGet()
        {
            MtdGroup = new MtdGroup();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _context.MtdGroup.AddAsync(MtdGroup);
            await _context.SaveChangesAsync();
            return Page();
        }

    }
}