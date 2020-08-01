using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers
{
    [Route("images")]
    [ApiController]
    [AllowAnonymous]
    public class ImagesController : ControllerBase
    {
        public readonly OrderMakerContext context;

        public ImagesController(OrderMakerContext context)
        {    
            this.context = context;
        }

        [HttpGet("logo.png")]
        public async Task<IActionResult> OnGetLogoAsync()
        {
            byte[] fileData = await context.MtdConfigFiles.Where(x => x.Id == 1).Select(x => x.FileData).FirstOrDefaultAsync();
            return new FileContentResult(fileData, "text/plain"); /// { FileDownloadName = "logo.png" };
        }

    }
}
