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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.DataHandler.Approval;
using Mtd.OrderMaker.Server.Models.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Store.Stack
{
    [ViewComponent(Name = "StoreStackEditor")]
    public class Editor : ViewComponent
    {
        private readonly OrderMakerContext _context;

        public Editor(OrderMakerContext orderMakerContext)
        {
            _context = orderMakerContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(MtdFormPartField field, DataSet dataSet)
        {

            MtdStoreStack mtdStoreStack = dataSet.Stack.Where(x => x.MtdFormPartField == field.Id).FirstOrDefault();

            string viewName = GetViewName(field.MtdSysType, dataSet.Parts.FirstOrDefault().MtdSysStyle);

            if (mtdStoreStack == null)
            {
                mtdStoreStack = new MtdStoreStack()
                {
                    MtdFormPartField = field.Id,
                    MtdFormPartFieldNavigation = field,
                    MtdStore = dataSet.Store.Id
                };
            }

            CheckStackForNull(mtdStoreStack);

            if (field.MtdSysType == 11)
            {
                var fieldForList = await _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                        .Where(x => x.MtdFormPartNavigation.MtdForm == field.MtdFormList.MtdForm & x.MtdSysType == 1)
                        .OrderBy(o => o.MtdFormPartNavigation.Sequence).ThenBy(o=>o.Sequence).FirstOrDefaultAsync();                

                IList<long> stackIds = await _context.MtdStoreStack.Where(x => x.MtdFormPartField == fieldForList.Id).Select(x => x.Id).ToListAsync();

                var dataList = await _context.MtdStoreStack
                    .Include(m => m.MtdStoreStackText)                    
                    .Where(x => stackIds.Contains(x.Id))
                    .Select(x => new { Id = x.MtdStore, Name = x.MtdStoreStackText.Register })
                    .OrderBy(x=>x.Name)
                    .ToListAsync();

                string idSelected = null;
                if (mtdStoreStack.MtdStoreLink != null) { idSelected = mtdStoreStack.MtdStoreLink.MtdStore; }
                ViewData[field.Id] = new SelectList(dataList, "Id", "Name", idSelected);
            }

            ViewData["TypeStyle"] = field.MtdFormPartNavigation.MtdSysStyle == 5 ? "Columns" : "Rows";

            return View(viewName, mtdStoreStack);
        }


        private string GetViewName(int type, int style)
        {

            string viewName;

            switch (type)
            {
                case 2: { viewName = "Integer"; break; }
                case 3: { viewName = "Decimal"; break; }
                case 4: { viewName = "Memo"; break; }
                case 5: { viewName = "Date"; break; }
                case 6: { viewName = "DateTime"; break; }
                case 7:
                case 8: { viewName = style == 5 ? "FileColumn" : "FileRow"; break; }
                case 11: { viewName = "ListForm"; break; }
                case 12: { viewName = "CheckBox"; break; }
                default: { viewName = "Text"; break; }
            };

            return viewName;
        }

        private void CheckStackForNull(MtdStoreStack mtdStoreStack)
        {

            if (mtdStoreStack.MtdStoreStackDate == null) { mtdStoreStack.MtdStoreStackDate = new MtdStoreStackDate(); }
            if (mtdStoreStack.MtdStoreStackDecimal == null) { mtdStoreStack.MtdStoreStackDecimal = new MtdStoreStackDecimal(); }
            if (mtdStoreStack.MtdStoreStackFile == null) { mtdStoreStack.MtdStoreStackFile = new MtdStoreStackFile(); }
            if (mtdStoreStack.MtdStoreStackInt == null) { mtdStoreStack.MtdStoreStackInt = new MtdStoreStackInt(); }
            if (mtdStoreStack.MtdStoreStackText == null) { mtdStoreStack.MtdStoreStackText = new MtdStoreStackText(); }
        }
    }
}
