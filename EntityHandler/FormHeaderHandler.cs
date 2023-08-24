using Mtd.OrderMaker.Server.Entity;
using System;

namespace Mtd.OrderMaker.Server.EntityHandler
{
    public static class FormHeaderHandler
    {
        public static string GetImageSrc(MtdFormHeader formHeader)
        {
            string imgSrc = string.Empty;
            if (formHeader != null)
            {
                string base64 = Convert.ToBase64String(formHeader.Image);
                imgSrc = string.Format("data:{0};base64,{1}", formHeader.ImageType, base64);
            }

            return imgSrc;
        }
    }
}
