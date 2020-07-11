using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Controllers.Index.Filter
{
    [Route("api/filter/extension")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class ExtendController : ControllerBase
    {
        private readonly OrderMakerContext context;
        private readonly UserHandler userHandler;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ExtendController(OrderMakerContext context, UserHandler userHandler, IStringLocalizer<SharedResource> localizer)
        {
            this.context = context;
            this.userHandler = userHandler;
            this._localizer = localizer;
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterScriptAsync()
        {
            var form = await Request.ReadFormAsync();
            string formId = form["form-id"];
            string scriptId = form["script-id"];
            bool isOk = int.TryParse(scriptId, out int id);
            if (!isOk) { return BadRequest(_localizer["Error: Bad request."]); }
            
            bool available = await userHandler.IsFilterAccessingAsync(User, id);
            if (!available) { return BadRequest(_localizer["Error: Bad request."]); }
            
            MtdFilterScript mtdFilterScript = await context.MtdFilterScript.FindAsync(id);
            mtdFilterScript.Apply = 1;
            context.MtdFilterScript.Update(mtdFilterScript);
            await context.SaveChangesAsync();

            return Ok();

        }
    }
}
