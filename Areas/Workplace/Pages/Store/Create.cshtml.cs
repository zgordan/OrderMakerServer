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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public CreateModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        [BindProperty]
        public MtdStore MtdStore { get; set; }
        public MtdForm MtdForm { get; set; }

        public async Task<IActionResult> OnGet(string idForm, string idStoreParent)
        {

            if (idForm == null)
            {
                return NotFound();
            }

            var user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isCreator = await _userHandler.IsCreator(user, idForm);

            if (!isCreator)
            {
                return Forbid();
            }

            MtdForm = await _context.MtdForm.FindAsync(idForm);

            MtdStore mtdStoreParent = await _context.MtdStore.FirstOrDefaultAsync(x => x.Id == idStoreParent);

            MtdStore = new MtdStore { MtdForm = MtdForm.Id, MtdFormNavigation = MtdForm, Parent = idStoreParent, ParentNavigation = mtdStoreParent };
            return Page();
        }

    }
}