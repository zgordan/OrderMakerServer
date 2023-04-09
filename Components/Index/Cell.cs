/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
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
