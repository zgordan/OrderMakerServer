using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Store;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using Mtd.OrderMaker.Server.Models.Store;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Store
{
    [ViewComponent(Name = "StoreList")]
    public class StoreList : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;

        public StoreList(OrderMakerContext orderMakerContext, UserHandler userHandler, IStringLocalizer<SharedResource> localizer)
        {
            _context = orderMakerContext;
            _userHandler = userHandler;
            this.localizer = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(string resultTarget, string resultValue, IList<MtdForm> relatedForms, string listid)
        {
            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            List<MTDSelectListItem> rfs = new List<MTDSelectListItem>();
            string selecteFormId = relatedForms.Select(x => x.Id).FirstOrDefault();

            if (resultTarget != null && resultTarget != string.Empty)
            {
                string formId = _context.MtdStore.Where(x => x.Id == resultValue).Select(x => x.MtdForm).FirstOrDefault();
                if (formId != null) { selecteFormId = formId; }
            }
            
            StoreListHandler storeList = new StoreListHandler(_context, selecteFormId, resultValue, user, _userHandler, localizer);

            foreach (var form in relatedForms)
            {
                rfs.Add(new MTDSelectListItem { Id = form.Id, Value = form.Name, Selectded = form.Id == selecteFormId });
            }

            await storeList.FillDataAsync();

            string imgSrc = string.Empty;
            if (storeList.MtdForm != null && storeList.MtdForm.MtdFormHeader != null)
            {
                string base64 = Convert.ToBase64String(storeList.MtdForm.MtdFormHeader.Image);
                imgSrc = String.Format("data:{0};base64,{1}", storeList.MtdForm.MtdFormHeader.ImageType, base64);
            }

            StoreListModel model = new StoreListModel
            {
                ListId = listid,
                ImgSrc = imgSrc,
                CurentForm = storeList.MtdForm,
                Store = storeList.StoreFields,
                PageNumber = storeList.PageNumber,
                PageCount = storeList.PageCount,
                PageLine = storeList.PageLine,
                Columns = storeList.Columns,
                ResultTarget = resultTarget,
                ResultValue = resultValue,
                ResultView = storeList.ResultView,
                RelatedForms = rfs,
            };

            return View(model);
        }

    }
}
