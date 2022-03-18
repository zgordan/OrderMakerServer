/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.EntityHandler.Filter;
using Mtd.OrderMaker.Server.EntityHandler.Stack;
using Mtd.OrderMaker.Server.Models.Index;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index
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

        public async Task<IViewComponentResult> InvokeAsync(string formId)
        {
            var user = await _userHandler.GetUserAsync(HttpContext.User);

            FilterHandler handlerFilter = new FilterHandler(_context, formId, user, _userHandler);
            Incomer incomer = await handlerFilter.GetIncomerDataAsync();
            TypeQuery typeQuery = await handlerFilter.GetTypeQueryAsync(user);
            OutFlow outFlow = await handlerFilter.GetStackFlowAsync(incomer, typeQuery);
            
            IList<MtdStore> mtdStores = outFlow.MtdStores;
            IList<string> storeIds = mtdStores.Select(s => s.Id).ToList();

            StackHandler handlerStack = new StackHandler(_context);
            IList<MtdStoreStack> mtdStoreStack = await handlerStack.GetStackAsync(storeIds, incomer.FieldIds);

            IList<MtdStoreApproval> mtdStoreApprovals = await _context.MtdStoreApproval.Where(x => storeIds.Contains(x.Id)).ToListAsync();
            List<ApprovalStore> approvalStores = await ApprovalHandler.GetStoreStatusAsync(_context, storeIds, user);

            MtdApproval mtdApproval = await _context.MtdApproval.Where(x => x.MtdForm == formId).FirstOrDefaultAsync();
            
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == formId);
            var mtdFormList = await ApprovalHandler.GetWaitStoreIds(_context, user, formId);
            int pending = mtdFormList.Count();

            RowsModelView rowsModel = new RowsModelView
            {
                FormId = formId,
                SearchNumber = incomer.SearchNumber,
                PageCount = outFlow.PageCount,
                MtdFormPartFields = incomer.FieldForColumn,
                MtdStores = mtdStores,
                MtdStoreStack = mtdStoreStack,
                WaitList = incomer.WaitList == 1,
                ShowDate = await handlerFilter.IsShowDate(),
                ShowNumber = await handlerFilter.IsShowNumber(),
                ApprovalStores = approvalStores,
                MtdApproval = mtdApproval,
                StoreIds = string.Join("&", storeIds),
                SearchText = filter == null ? string.Empty : filter.SearchText,
                Pending = pending,
                IsCreator = await _userHandler.IsCreator(user, formId),
                PageSize = filter.PageSize,
                PageCurrent = filter.Page,
                
            };

            return View("Default", rowsModel);
        }
    }
}
