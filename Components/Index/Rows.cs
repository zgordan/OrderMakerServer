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
using Mtd.OrderMaker.Web.DataHandler.Filter;
using Mtd.OrderMaker.Web.DataHandler.Stack;
using Mtd.OrderMaker.Web.Models.Index;
using Mtd.OrderMaker.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components.Index
{
    [ViewComponent(Name = "IndexRows")]
    public class Rows : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;
        public int pageCount = 0;


        public Rows(OrderMakerContext orderMakerContext, UserHandler userHandler)
        {
            _context = orderMakerContext;
            _userHandler = userHandler;
        }

        public async Task<IViewComponentResult> InvokeAsync(string idForm)
        {
            var user = await _userHandler.GetUserAsync(HttpContext.User);
            List<string> partIds = await _userHandler.GetAllowPartsForView(user, idForm);
                                
            FilterHandler handlerFilter = new FilterHandler(_context, idForm, user, _userHandler);
            Incomer incomer = await handlerFilter.GetIncomerDataAsync();
            TypeQuery typeQuery = await handlerFilter.GetTypeQueryAsync();
            OutFlow outFlow = await handlerFilter.GetStackFlowAsync(incomer, typeQuery);
            IList<MtdStore> mtdStore = outFlow.MtdStores;

            decimal count = (decimal) outFlow.Count / incomer.PageSize;
            pageCount = Convert.ToInt32(Math.Ceiling(count));
            pageCount = pageCount == 0 ? 1 : pageCount;

            IList<string> storeIds = mtdStore.Select(s => s.Id).ToList();
            IList<string> fieldIds =  fieldIds = incomer.FieldForColumn.Select(x => x.Id).ToList();

            IList<string> allowFiieldIds = await _context.MtdFormPartField.Where(x => partIds.Contains(x.MtdFormPart)).Select(x => x.Id).ToListAsync();
            fieldIds = allowFiieldIds.Where(x=>fieldIds.Contains(x)).ToList();

            StackHandler handlerStack = new StackHandler(_context);
            IList<MtdStoreStack> mtdStoreStack = await handlerStack.GetStackAsync(storeIds, fieldIds);

            IList<MtdStoreApproval> mtdStoreApprovals = await _context.MtdStoreApproval.Where(x => storeIds.Contains(x.Id)).ToListAsync();           
            List<ApprovalStore> approvalStores = await ApprovalHandler.GetStoreStatusAsync(_context, storeIds, user);

            bool isApprovalForm = await ApprovalHandler.IsApprovalFormAsync(_context, idForm);

            RowsModelView rowsModel = new RowsModelView
            {
                IdForm = idForm,
                SearchNumber = incomer.SearchNumber,
                PageCount = pageCount,
                MtdFormPartFields = incomer.FieldForColumn.Where(x => fieldIds.Contains(x.Id)).ToList(),
                MtdStores = mtdStore,
                MtdStoreStack = mtdStoreStack,
                WaitList = incomer.WaitList == 1 ? true : false,
                ShowDate = await handlerFilter.IsShowDate(),
                ShowNumber = await handlerFilter.IsShowNumber(),
                ApprovalStores = approvalStores,
                IsAppromalForm = isApprovalForm

            };

            return View("Default", rowsModel);
        }

    }



}
