using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers
{
    [Route("api/common")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class CommonController : ControllerBase
    {

        public readonly UserHandler userHandler;

        public CommonController(UserHandler userHandler)
        {
            this.userHandler = userHandler;
        }

        [AllowAnonymous]
        [HttpPost("password/generate")]
        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPasswordGenerateAsync()
        {
            string data = await Task.Run(()=> userHandler.GeneratePassword());

            return Ok(new JsonResult(data));
        }
    }
}
