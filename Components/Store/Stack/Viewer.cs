/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Store;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Store.Stack
{
    [ViewComponent(Name = "StoreStackViewer")]
    public class Viewer : ViewComponent
    {


        public Viewer() { }

        public async Task<IViewComponentResult> InvokeAsync(MtdFormPartField field, Warehouse warehouse)
        {

            MtdStoreStack mtdStoreStack = await GetMtdStoreStackAsync(field, warehouse);
            mtdStoreStack ??= new MtdStoreStack();
            string viewName = await GetViewNameAsync(field.MtdSysType);

            ViewData["typeStyle"] = field.MtdFormPartNavigation.MtdSysStyle == 5 ? "Columns" : "Rows";

            return View(viewName, mtdStoreStack);
        }


        private static MtdStoreStack GetMtdStoreStack(MtdFormPartField field, Warehouse wh)
        {
            return wh.Stack.Where(x => x.MtdFormPartField == field.Id).FirstOrDefault();
        }

        private static async Task<MtdStoreStack> GetMtdStoreStackAsync(MtdFormPartField field, Warehouse wh)
        {
            return await Task.Run(() => GetMtdStoreStack(field, wh));
        }

        private static string GetViewName(int type)
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

        private static async Task<string> GetViewNameAsync(int type)
        {
            return await Task.Run(() => GetViewName(type));
        }

    }
}
