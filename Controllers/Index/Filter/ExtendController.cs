using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public ExtendController(OrderMakerContext context, UserHandler userHandler)
        {
            this.context = context;
            this.userHandler = userHandler;
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterScriptAsync()
        {
            var form = await Request.ReadFormAsync();
            string formId = form["form-id"];
            string scriptId = form["script-id"];
            bool isOk = int.TryParse(scriptId, out int id);
            if (!isOk) { return BadRequest("Error: Bad request."); }
            
            bool available = await userHandler.IsFilterAccessingAsync(User, id);
            if (!available) { return BadRequest("Error: Bad request."); }
            
            MtdFilterScript mtdFilterScript = await context.MtdFilterScript.FindAsync(id);
            mtdFilterScript.Apply = 1;
            context.MtdFilterScript.Update(mtdFilterScript);
            await context.SaveChangesAsync();

            return Ok();

        }
    }
}
