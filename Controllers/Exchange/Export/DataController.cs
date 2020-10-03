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
using Mtd.OrderMaker.Server.Models.Store;
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
        public async Task<IActionResult> OnPostExportAsyns(string formId, string usr, string pwd, int page = 1, string dateStart = null)
        {

            /*check access*/
            WebAppUser user = await userHandler.FindByNameAsync(usr);
            if (user == null) { return BadRequest("Invalid user."); }

            bool isOk = await userHandler.CheckPasswordAsync(user, pwd);
            if (!isOk) { return BadRequest("Invalid user."); }

            isOk = await userHandler.IsViewer(user, formId);
            if (!isOk) { return BadRequest("Access denied."); }

            FilterHandler filterHandler = new FilterHandler(context, formId, user, userHandler);
            TypeQuery typeQuery = TypeQuery.empty;

            IList<MtdFormPartField> fields = await filterHandler.GetFieldsFilterAsync();
            fields = fields.Where(x => x.MtdSysType != 7 && x.MtdSysType != 9).ToList();

            Incomer incomer = new Incomer
            {
                FormId = formId,
                SearchNumber = string.Empty,
                SearchText = string.Empty,
                Page = page,
                PageSize = 250,
                FieldForColumn = fields,
                WaitList = 0,
            };

            OutFlow outFlow = await filterHandler.GetStackFlowAsync(incomer, typeQuery);
            StackHandler handlerStack = new StackHandler(context);

            List<string> storeIds = new List<string>();

            if (dateStart == null)
            {
                storeIds = outFlow.MtdStores.Select(x => x.Id).ToList();

            } else
            {
                isOk = DateTime.TryParseExact(dateStart, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                if (!isOk) { return BadRequest("Invakid date format."); }

                storeIds = await context.MtdLogDocument.Where(x => x.TimeCh > dateTime).GroupBy(x => x.MtdStore).Select(x => x.Key).ToListAsync();
                
            }


            IList<MtdStoreStack> mtdStoreStack = await handlerStack.GetStackAsync(storeIds, incomer.FieldIds);

            List<StoreListFields> storeFields = new List<StoreListFields>();

            foreach (string storeId in storeIds)
            {
                MtdStore mtdStore = outFlow.MtdStores.Where(x => x.Id == storeId).FirstOrDefault();

                string number = mtdStore.Sequence.ToString("D9");
                string date = mtdStore.Timecr.ToShortDateString();
                StoreListFields store = new StoreListFields()
                {
                    StoreId = storeId,
                    Fields = new List<StoreListField>() {

                         new StoreListField{ Id = "store-id", Name = localizer["ID"], Value = storeId},
                         new StoreListField{ Id = "store-name", Name = localizer["Name"], Value = $"{localizer["No."]} {number} {localizer["at"]} {date}" },
                         new StoreListField{ Id = "store-number", Name = localizer["Number"], Value = number},
                         new StoreListField{ Id = "store-date", Name = localizer["Date"], Value = date},
                    }
                };

                foreach (MtdFormPartField field in incomer.FieldForColumn)
                {
                    MtdStoreStack stack = mtdStoreStack.Where(x => x.MtdStore == storeId && x.MtdFormPartField == field.Id).FirstOrDefault();
                    string value = handlerStack.GetValueAsString(stack, field);
                    store.Fields.Add(new StoreListField { Id = field.Id, SysType = field.MtdSysType.ToString(), Name = field.Name, Value = value });
                }

                storeFields.Add(store);
            }

            return Ok(new JsonResult(new { pageCount = outFlow.PageCount, store = storeFields }));
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
