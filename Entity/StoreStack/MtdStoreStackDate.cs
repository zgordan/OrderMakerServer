/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdStoreStackDate
    {
        public long Id { get; set; }
        public DateTime Register { get; set; }

        public virtual MtdStoreStack IdNavigation { get; set; }
    }
}
