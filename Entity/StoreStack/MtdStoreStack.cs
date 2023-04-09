/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

*/

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdStoreStack
    {
        public long Id { get; set; }
        public string MtdStore { get; set; }
        public string MtdFormPartField { get; set; }        
        public virtual MtdFormPartField MtdFormPartFieldNavigation { get; set; }
        public virtual MtdStore MtdStoreNavigation { get; set; }
        public virtual MtdStoreLink MtdStoreLink { get; set; }
        public virtual MtdStoreStackDate MtdStoreStackDate { get; set; }
        public virtual MtdStoreStackDecimal MtdStoreStackDecimal { get; set; }
        public virtual MtdStoreStackFile MtdStoreStackFile { get; set; }
        public virtual MtdStoreStackInt MtdStoreStackInt { get; set; }
        public virtual MtdStoreStackText MtdStoreStackText { get; set; }
    }
}
