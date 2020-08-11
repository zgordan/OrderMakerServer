using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Filter;
using Mtd.OrderMaker.Server.EntityHandler.Stack;
using Mtd.OrderMaker.Server.EntityHandler.Store;
using Mtd.OrderMaker.Server.Models.Store;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Controllers.Store
{
    [Route("api/store")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class StoreListController : ControllerBase
    {
        private readonly OrderMakerContext context;
        private readonly UserHandler userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;

        public StoreListController(OrderMakerContext context, UserHandler userHandler, IStringLocalizer<SharedResource> localizer)
        {
            this.context = context;
            this.userHandler = userHandler;
            this.localizer = localizer;
        }

        [HttpPost("list")]
        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostStoreAsync()
        {
            IFormCollection form = await Request.ReadFormAsync();
            string formId = form["form-id"];
            string searchText = form["search-text"];
            string searchNumber = form["search-number"];
            string searchPage = form["search-page"];
            string selectedId = form["selected-id"];

            bool isOk = int.TryParse(searchPage, out int pageNumber);
            if (!isOk) { pageNumber = 1; }

            isOk = int.TryParse(searchNumber, out int number);
            if (!isOk) { number = 0; }
           
            WebAppUser user = await userHandler.GetUserAsync(HttpContext.User);

            StoreListHandler storeList = new StoreListHandler(context, formId, selectedId, user, userHandler, localizer);
            storeList.SetFilterText(searchText);
            storeList.SetFilterNumber(number);
            storeList.SetPageNumber(pageNumber);
            await storeList.FillDataAsync();

            string imgSrc = string.Empty;
            if (storeList.MtdForm.MtdFormHeader != null)
            {
                var base64 = Convert.ToBase64String(storeList.MtdForm.MtdFormHeader.Image);
                imgSrc = String.Format("data:{0};base64,{1}", storeList.MtdForm.MtdFormHeader.ImageType, base64);
            }
                                    
            return new JsonResult(new { 

                formId, 
                formName = storeList.MtdForm.Name, 
                formImg = imgSrc,
                pageNumber = storeList.PageNumber, 
                pageCount = storeList.PageCount, 
                pageLine = storeList.PageLine, 
                columns = storeList.Columns, 
                store = storeList.StoreFields });
        }

    }



}
