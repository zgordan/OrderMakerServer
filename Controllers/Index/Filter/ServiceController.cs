using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Controllers.Index.Filter
{
    [Route("api/filter/service")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class ServiceController : ControllerBase
    {
        private readonly OrderMakerContext context;
        private readonly UserHandler userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;

        public ServiceController(OrderMakerContext context, UserHandler userHandler, IStringLocalizer<SharedResource> localizer)
        {
            this.context = context;
            this.userHandler = userHandler;
            this.localizer = localizer;
        }

        [HttpPost("add/date")]
        [Produces("application/json")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddDate()
        {
            var form = await Request.ReadFormAsync();

            string formId = form["form-id"];
            string dateStart = form["date-start"];
            string dateFinish = form["date-finish"];
            string dateFormat = form["date-format"];

            WebAppUser user = await userHandler.GetUserAsync(User);
            MtdFilter filter = await userHandler.GetFilterAsync(User, formId);

            bool isOkDateStart = DateTime.TryParseExact(dateStart, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart);
            bool isOkDateFinish = DateTime.TryParseExact(dateFinish, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeFinish);

            //bool isOkDateStart = DateTime.TryParse(dateStart, out DateTime dateTimeStart);
            //bool isOkDateFinish = DateTime.TryParse(dateFinish, out DateTime dateTimeFinish);

            if (isOkDateStart && isOkDateFinish)
            {
                MtdFilterDate mtdFilterDate = new MtdFilterDate
                {
                    Id = filter.Id,
                    DateStart = dateTimeStart,
                    DateEnd = dateTimeFinish
                };

                bool isExists = await context.MtdFilterDate.Where(x => x.Id == filter.Id).AnyAsync();

                if (isExists)
                {
                    context.MtdFilterDate.Update(mtdFilterDate);
                }
                else
                {
                    await context.MtdFilterDate.AddAsync(mtdFilterDate);
                }

                await context.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpPost("add/owner")]
        [Produces("application/json")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddOwner()
        {
            var form = await Request.ReadFormAsync();
            string formId = form["form-id"];
            string ownerId = form["owner-id"];
            
            WebAppUser user = await userHandler.GetUserAsync(User);
            MtdFilter filter = await userHandler.GetFilterAsync(User, formId);

            MtdFilterOwner mtdFilterOwner = new MtdFilterOwner
            {
                Id = filter.Id,
                OwnerId = ownerId                 
            };

            bool isExists = await context.MtdFilterOwner.Where(x => x.Id == filter.Id).AnyAsync();

            if (isExists)
            {
                context.MtdFilterOwner.Update(mtdFilterOwner);
            }
            else
            {
                await context.MtdFilterOwner.AddAsync(mtdFilterOwner);
            }

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("add/related")]
        [Produces("application/json")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddRelated()
        {
            IFormCollection requestForm = await Request.ReadFormAsync();
            string formId = requestForm["form-id"];
            string docBasedId = requestForm["document-based-id"];
            string relatedNumber = requestForm["related-number"];

            bool isOk = int.TryParse(relatedNumber, out int docNumber);
            if (!isOk) { return BadRequest(localizer["Invalid number format."]); }
                        
            WebAppUser user = await userHandler.GetUserAsync(HttpContext.User);
            MtdFilter filter = await userHandler.GetFilterAsync(User, formId);

            if (filter == null) { return BadRequest(localizer["Error! No filter defined."]); }

            MtdFilterRelated mtdFilterRelated = new MtdFilterRelated
            {
                Id = filter.Id,
                FormId = docBasedId,
                DocBasedNumber = docNumber                
            };

            bool isExists = await context.MtdFilterRelated.Where(x => x.Id == filter.Id).AnyAsync();

            if (isExists)
            {
                context.MtdFilterRelated.Update(mtdFilterRelated);
            }
            else
            {
                await context.MtdFilterRelated.AddAsync(mtdFilterRelated);
            }

            await context.SaveChangesAsync();

            return Ok();
        }
    }

}
