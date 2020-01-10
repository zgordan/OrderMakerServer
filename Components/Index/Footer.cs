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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Models.Index;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index
{
    [ViewComponent(Name = "IndexFooter")]
    public class Footer : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;

        public Footer(OrderMakerContext orderMakerContext, UserManager<WebAppUser> userManager)
        {
            _context = orderMakerContext;
            _userManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync(string idForm, int pageCount)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            int pageSize = 10;
            int page = 1;
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == idForm);
            if (filter != null) { page = filter.Page; pageSize = filter.PageSize; }

            FooterModelView model = new FooterModelView { IdForm = idForm, Page = page, PageSize = pageSize, PageCount = pageCount };

            return View("Default", model);
        }
    }
}
