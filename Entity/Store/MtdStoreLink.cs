/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdStoreLink
    {
        public long Id { get; set; }
        public string MtdStore { get; set; }
        public string Register { get; set; }

        public virtual MtdStoreStack IdNavigation { get; set; }
        public virtual MtdStore MtdStoreNavigation { get; set; }
    }
}
