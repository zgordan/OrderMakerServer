/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdFormPartHeader
    {
        public string Id { get; set; }
        public byte[] Image { get; set; }
        public string ImageType { get; set; }
        public int ImageSize { get; set; }

        public virtual MtdFormPart IdNavigation { get; set; }
    }
}
