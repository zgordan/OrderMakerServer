using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components
{
    [ViewComponent(Name = "MTDImgViewer")]
    public class MTDImgViewer : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync (int width=-1, int height=-1, int maxWidth=-1, int maxHeight=-1, byte[] imgArray=null)
        {
            MTDImgViewerModel model = await Task.Run(()=> new MTDImgViewerModel { 
                Width = width, 
                Height = height, 
                MaxWidth = maxWidth,
                MaxHeight = maxHeight,
                ImgArray = imgArray 
            });
            return View(model);
        }
    }

    public class MTDImgViewerModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public byte[] ImgArray { get; set; }
    }
}
