/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Store;
using System.Linq;

namespace Mtd.OrderMaker.Server.Components.Store.Part
{
    [ViewComponent (Name = "StorePartViewer")]
    public class Viewer : ViewComponent
    {
        public IViewComponentResult Invoke(MtdFormPart part, Warehouse model)
        {
            Warehouse warehouse = new Warehouse()
            {               
                Store = model.Store,
                Parts = model.Parts.Where(x=>x.Id == part.Id).ToList(),
                Stack = model.Stack,
                Fields = model.Fields.Where(x => x.MtdFormPart == part.Id).OrderBy(x => x.Sequence).ToList()
            };

            string viewName = part.MtdSysStyle == 5 ? "Columns" : "Rows";
                                   
            return View(viewName,warehouse);
        }
    }
}
