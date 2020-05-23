using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Controls
{
    [ViewComponent(Name = "MTDTextField")]
    public class MTDTextField : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            return await Task.Run(() => View());
        }
    }
}
