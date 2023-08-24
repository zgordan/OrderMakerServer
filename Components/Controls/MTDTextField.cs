using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Models.Controls.MTDTextField;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Controls
{
    [ViewComponent(Name = "MTDTextField")]
    public class MTDTextField : ViewComponent
    {
        private readonly IStringLocalizer<SharedResource> Localizer;

        public MTDTextField(IStringLocalizer<SharedResource> localizer)
        {
            Localizer = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(MTDTextFieldTags tags)
        {
            MTDTextFieldTagsModel model = new MTDTextFieldTagsModel(tags);

            LocalizerModel(model);

            string viewName = model.MTDTexFieldView.ToString();
            return await Task.Run(() => View(viewName, model));
        }


        private void LocalizerModel(MTDTextFieldTagsModel model)
        {
            if (model.Label != null && model.LabelLocalized)
            {
                model.Label = Localizer[$"{model.Label}"];
            }

            if (model.Placeholder != null && model.PlaceholderLocalized)
            {
                model.Placeholder = Localizer[$"{model.Placeholder}"];
            }

            if (model.HelperText != null && model.HelperTextLocalizer)
            {
                model.HelperText = Localizer[$"{model.HelperText}"];
            }

            if (model.HelperError != null && model.HelperErrorLocalizer)
            {
                model.HelperError = Localizer[$"{model.HelperError}"];
            }
        }
    }
}
