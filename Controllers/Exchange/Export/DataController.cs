using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Filter;
using Mtd.OrderMaker.Server.EntityHandler.Stack;
using Mtd.OrderMaker.Server.EntityHandler.Store;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Exchange.Export
{
    [Route("api/exchange")]
    [ApiController]    
    public class DataController : ControllerBase
    {
        private readonly OrderMakerContext context;
        private readonly IOptions<ConfigSettings> options;
        private readonly UserHandler userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;

        public DataController(OrderMakerContext context, IOptions<ConfigSettings> options, UserHandler userHandler, IStringLocalizer<SharedResource> localizer)
        {
            this.context = context;
            this.options = options;
            this.userHandler = userHandler;
            this.localizer = localizer;
            
        }

        [HttpGet("export")]
        [Produces("application/json")]     
        public async Task<IActionResult> OnPostExportAsyns(string formId, string usr, string pwd, string dateStart)
        {
            
            /*check access*/
            WebAppUser user = await userHandler.FindByNameAsync(usr);
            if (user == null) { return BadRequest("Invalid user."); }
            bool isOk = await userHandler.CheckPasswordAsync(user, pwd);
            if (!isOk) { return BadRequest("Invalid user."); }

            isOk = DateTime.TryParseExact(dateStart, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            if (!isOk) { return BadRequest("Invalid format date."); }

            isOk = await userHandler.IsViewer(user, formId);
            if (!isOk) { return BadRequest("Access denied."); }

            StoreListHandler storeList = new StoreListHandler(context, formId, "", user, userHandler, localizer);
            await storeList.FillDataAsync();

            return Ok(new JsonResult(storeList.StoreFields));
        }


        private bool CheckToken(string pk, string token)
        {
            string tokenStr = $"{options.Value.SecretKey}{pk}";

            byte[] data = Encoding.UTF8.GetBytes(tokenStr);
            byte[] result;
            SHA512 shaM = new SHA512Managed();
            result = shaM.ComputeHash(data);

            var hashedInputStringBuilder = new System.Text.StringBuilder(128);
            foreach (var b in result) { hashedInputStringBuilder.Append(b.ToString("X2")); }
            string tokenSHA = hashedInputStringBuilder.ToString();

            return tokenSHA.ToUpper() != token.ToUpper();
        }
    }
}
