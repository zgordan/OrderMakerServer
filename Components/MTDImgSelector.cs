using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components
{

    [ViewComponent(Name = "MTDImgSelector")]
    public class MTDImgSelector : ViewComponent
    {

        private static bool CheckDelete(string codeForm, HttpRequest request)
        {

            string idCheckBox = $"{codeForm}-delete";
            if (request.Form[idCheckBox].FirstOrDefault() == null || request.Form[idCheckBox].FirstOrDefault() == "false")
            {
                return false;
            }

            return true;
        }

        private static async Task<byte[]> GetImageAsync(string codeForm, HttpRequest request)
        {

            string idInput = $"{codeForm}-file-upload-input";
            IFormFile file = request.Form.Files.FirstOrDefault(x => x.Name == idInput);
            if (file != null)
            {
                byte[] streamArray = new byte[file.Length];
                await file.OpenReadStream().ReadAsync(streamArray, 0, streamArray.Length);
                return streamArray;
            }

            return null;
        }


        public static async Task<MTDImgSModify> ImageModifyAsync(string codeForm, HttpRequest request)
        {

            var imgArray = await MTDImgSelector.GetImageAsync(codeForm, request);
            bool delCommand = MTDImgSelector.CheckDelete(codeForm, request);

            MTDImgSModify imgSModify = new MTDImgSModify { Image = null, Modify = false };

            if (delCommand)
            {
                imgSModify.Image = null;
                imgSModify.Modify = true;
            }

            if (imgArray != null && !delCommand)
            {
                imgSModify.Image = imgArray;
                imgSModify.Modify = true;
            }

            if (imgArray == null && !delCommand)
            {
                imgSModify.Image = null;
                imgSModify.Modify = false;
            }

            return imgSModify;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, bool required = false, byte[] imgArray = null)
        {
            MTDImgSelectorModel ism = await Task.Run(()=> new MTDImgSelectorModel { Id = id, ImgArray = imgArray, Required = required });
            return View(ism);
        }
    }

    public class MTDImgSModify
    {
        public byte[] Image { get; set; }
        public bool Modify { get; set; }
    }

    public class MTDImgSelectorModel
    {
        public string Id { get; set; }
        public byte[] ImgArray { get; set; }
        public bool Required { get; set; }
    }
}
