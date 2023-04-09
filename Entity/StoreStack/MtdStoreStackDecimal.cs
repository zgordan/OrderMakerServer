/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

*/


namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdStoreStackDecimal
    {
        public long Id { get; set; }
        public decimal Register { get; set; }
        public virtual MtdStoreStack IdNavigation { get; set; }
    }
}
