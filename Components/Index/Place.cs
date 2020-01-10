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
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index
{
    [ViewComponent(Name = "IndexPlace")]
    public class Place : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;

        public Place(OrderMakerContext context, UserManager<WebAppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string idForm)
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            bool isExists = await _context.MtdFilter.Where(x => x.IdUser == user.Id).AnyAsync();

            if (!isExists)
            {
                MtdFilter mtdFilter = new MtdFilter
                {
                    IdUser = user.Id,
                    MtdForm = idForm,
                    Page = 1,
                    PageSize = 10,
                    SearchNumber = "",
                    SearchText = ""
                };
                await _context.MtdFilter.AddAsync(mtdFilter);
                await _context.SaveChangesAsync();
            }

            PlaceModelView model = new PlaceModelView { IdForm = idForm };
            return View("Default", model);
        }
    }
}
