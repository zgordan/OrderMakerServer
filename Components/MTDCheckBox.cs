﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components
{
    [ViewComponent(Name = "MTDCheckbox")]
    public class MTDCheckbox : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string id, string label, bool result)
        {
            MTDCheckboxModel boxModel = await Task.Run(() => new MTDCheckboxModel { Id = id, Label = label, Result = result });
            return View(boxModel);
        }

        public static async Task<bool> GetResultAsync(string id, HttpRequest request)
        {
            bool result = false;
            StringValues value = await Task.Run(()=> request.Form[$"{id}-mtd-checkbox-input"]);

            if (!StringValues.IsNullOrEmpty(value) && value != "false") { result = true; }

            return result;
        }

    }

    public class MTDCheckboxModel
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool Result { get; set; }
    }
}
