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
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Models.Index;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index
{
    [ViewComponent(Name = "IndexCell")]
    public class Cell : ViewComponent
    {
        private readonly OrderMakerContext _context;

        public Cell(OrderMakerContext orderMakerContext)
        {
            _context = orderMakerContext;            
        }

        public async Task<IViewComponentResult> InvokeAsync(IList<MtdStoreStack> stack, string idStore, string idField, int idType)
        {

            string viewName = await GetViewNameAsync(idType);
            MtdStoreStack storeStack = await GetStoreStackAsync(stack, idStore, idField);
            
            CellModelView cellModel = new CellModelView
            {
                MtdStoreStack = storeStack ?? new MtdStoreStack()
            };

            return View(viewName, cellModel);
        }


        private async Task<MtdStoreStack> GetStoreStackAsync(IList<MtdStoreStack> stack, string idStore, string idField) {
            return await Task.Run(()=> stack.Where(x => x.MtdStore == idStore && x.MtdFormPartField == idField).FirstOrDefault());
        }

        private string GetViewName(int idType) {

            string viewName;

            switch (idType)
            {
                case 2: { viewName = "Integer"; break; }
                case 3: { viewName = "Decimal"; break; }
                case 4: { viewName = "Memo"; break; }
                case 5: { viewName = "Date"; break; }
                case 6: { viewName = "DateTime"; break; }
                case 7: { viewName = "File"; break; }
                case 8: { viewName = "Picture"; break; }
                case 10: { viewName = "Time"; break; }
                case 11: { viewName = "List"; break; }
                case 12: { viewName = "CheckBox"; break; }
                case 13: { viewName = "Link"; break; }

                default:
                    {
                        viewName = "Text";
                        break;
                    }
            }

            return viewName;
        }

        private async Task<string> GetViewNameAsync(int idType) {
            return await Task.Run(() => GetViewName(idType));
        }
    }
}
