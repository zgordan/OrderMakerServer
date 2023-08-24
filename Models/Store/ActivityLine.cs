using System;

namespace Mtd.OrderMaker.Server.Models.Store
{
    public class ActivityLine
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string UserId { get; set; }
        public DateTime TimeCr { get; set; }
        public string ImgSrc { get; set; }
        public string Comment { get; set; }

    }
}
