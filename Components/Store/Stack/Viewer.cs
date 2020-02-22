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
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Models.Store;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Store.Stack
{
    [ViewComponent(Name = "StoreStackViewer")]
    public class Viewer : ViewComponent
    {
        private readonly OrderMakerContext _context;

        public Viewer(OrderMakerContext orderMakerContext)
        {
            _context = orderMakerContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(MtdFormPartField field, DataSet dataSet)
        {

            MtdStoreStack mtdStoreStack = await GetMtdStoreStackAsync(field, dataSet);
            if (mtdStoreStack == null) { mtdStoreStack = new MtdStoreStack(); }
            string viewName = await GetViewNameAsync(field.MtdSysType,dataSet.Parts.FirstOrDefault().MtdSysStyle);

            ViewData["typeStyle"] = field.MtdFormPartNavigation.MtdSysStyle == 5 ? "Columns" : "Rows" ;

            return View(viewName, mtdStoreStack);
        }


        private MtdStoreStack GetMtdStoreStack(MtdFormPartField field, DataSet dataSet) {
            return dataSet.Stack.Where(x => x.MtdFormPartField == field.Id).FirstOrDefault();
        }

        private async Task<MtdStoreStack> GetMtdStoreStackAsync(MtdFormPartField field, DataSet dataSet) {
            return  await Task.Run(() => GetMtdStoreStack(field,dataSet));
        }

        private string GetViewName(int type, int style )
        {
            string viewName;
            switch (type)
            {
                case 2: { viewName = "Integer"; break; }
                case 3: { viewName = "Decimal"; break; }
                case 4: { viewName = "Memo"; break; }
                case 5: { viewName = "Date"; break; }
                case 6: { viewName = "DateTime"; break; }
                case 7: { viewName = "File"; break; }
                case 8: { viewName = "Picture"; break; }
                case 11: { viewName = "ListForm"; break; }
                case 12: { viewName = "CheckBox"; break; }
                case 13: { viewName = "Link"; break; }
                default: { viewName = "Text"; break; }
            };
                       
            return viewName;
        }

        private async Task<string> GetViewNameAsync(int type, int style) {
            return await Task.Run(() => GetViewName(type, style));
        }

    }
}
