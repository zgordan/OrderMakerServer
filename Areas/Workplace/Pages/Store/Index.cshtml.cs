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
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store
{
    public class IndexModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;
        private readonly LimitSettings limit;

        public IndexModel(OrderMakerContext context, UserHandler userHandler, IOptions<LimitSettings> limit)
        {
            _context = context;
            _userHandler = userHandler;
            this.limit = limit.Value;
        }

        public MtdForm MtdForm { get; set; }
        public bool ExportToExcel { get; set; }

        public async Task<IActionResult> OnGetAsync(string indexForm)
        {
            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isViewer = await _userHandler.CheckUserPolicyAsync(user, indexForm, RightsType.ViewAll);
            bool GroupRight = await _userHandler.CheckUserPolicyAsync(user, indexForm, RightsType.ViewGroup);
            bool OwnerRight = await _userHandler.CheckUserPolicyAsync(user, indexForm, RightsType.ViewOwn);            

            if (!isViewer & !OwnerRight & !GroupRight)
            {
                return Forbid(); 
            }

            MtdForm = await _context.MtdForm.FindAsync(indexForm);

            if (MtdForm == null)
            {
                return NotFound();
            }

            bool exporter = await _userHandler.CheckUserPolicyAsync(user, indexForm, RightsType.ExportToExcel);
            ExportToExcel = limit.ExportExcel == 1 && exporter;

            ViewData["FormId"] = indexForm;
            return Page();
        }

    }
}
