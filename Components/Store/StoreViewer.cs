using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Entity;
using NPOI.SS.Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Store
{
    public class StoreViewerModel
    {
        public string ImgSrc { get; set; }
        public string FormName { get; set; }
        public string DocName { get; set; }

        public string ViewerId { get; set; }
    }

    [ViewComponent(Name = "StoreViewer")]
    public class StoreViewer : ViewComponent
    {
        private readonly OrderMakerContext context;
        private readonly IStringLocalizer<SharedResource> localizer;

        public StoreViewer(OrderMakerContext context, IStringLocalizer<SharedResource> localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(string storeId, string viewerId)            
        {
            if (storeId == null)
            {
                StoreViewerModel nullModel = new StoreViewerModel
                {
                    ImgSrc = string.Empty,
                    FormName = string.Empty,
                    DocName = string.Empty,
                    ViewerId = viewerId
                };

                return View(nullModel);
            }

            MtdStore mtdStore = await context.MtdStore.FindAsync(storeId);
            MtdForm mtdForm = await context.MtdForm.Include(x => x.MtdFormHeader).Where(x => x.Id == mtdStore.MtdForm).FirstOrDefaultAsync();

            string imgSrc = string.Empty;
            if (mtdForm.MtdFormHeader != null)
            {
                string base64 = Convert.ToBase64String(mtdForm.MtdFormHeader.Image);
                imgSrc = String.Format("data:{0};base64,{1}", mtdForm.MtdFormHeader.ImageType, base64);
            }


            StoreViewerModel model = new StoreViewerModel
            {
                ImgSrc = imgSrc,
                FormName = mtdForm.Name,
                DocName = $"{localizer["No."]} {mtdStore.Sequence:D9} {localizer["at"]} {mtdStore.Timecr.ToShortDateString()}",
                ViewerId = viewerId
            };

            return View(model);
        }
    }
}
