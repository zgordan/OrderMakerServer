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
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Approval;
using Mtd.OrderMaker.Web.Models.Index;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components.Index
{
    [ViewComponent(Name = "IndexHeader")]
    public class Header : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;

        public Header(OrderMakerContext orderMakerContext, UserManager<WebAppUser> userManager)
        {
            _context = orderMakerContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string idForm) {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var mtdFormList =  await ApprovalHandler.GetWaitStoreIds(_context, user, idForm);
            int pending = mtdFormList.Count();

            string searchText = "";
            bool whiteList = false;
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x=>x.IdUser == user.Id && x.MtdForm==idForm);
            if (filter != null) {
                searchText = filter.SearchText;
                whiteList = filter.WaitList == 0 ? false : true;
            }

            HeaderModelView headerModelView = new HeaderModelView
            {
                IdForm = idForm,
                SearchText = searchText,
                WaitList = whiteList,
                Pending = pending,
                IsApprovalForm = await ApprovalHandler.IsApprovalFormAsync(_context, idForm)

            };

            return View("Default", headerModelView);
        }
    }
}
