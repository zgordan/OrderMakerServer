using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;

namespace Mtd.OrderMaker.Server.Components.Controls
{
    
    [ViewComponent(Name = "MTDSelectList")]
    public class MTDSelectList : ViewComponent
    {
        private readonly IStringLocalizer<SharedResource> Localizer;
        public MTDSelectList(IStringLocalizer<SharedResource> localizer)
        {
            Localizer = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(MTDSelectListTags tags)
        {
            MTDSelectListTagsModel model = new MTDSelectListTagsModel(tags);
            LocalizerModel(model);

            string viewName = model.MTDSelectListView.ToString();

            return await Task.Run(() => View(viewName,model));
        }


        private void LocalizerModel(MTDSelectListTagsModel model) {
            if (model.Label != null && model.LabelLocalized)
            {
                model.Label = Localizer[$"{model.Label}"];
            }

            model.Items.ForEach((item =>
            {
                if (item.Localized)
                {
                    item.Value = Localizer[$"{item.Value}"];
                }
            }));
        }
    }
}
