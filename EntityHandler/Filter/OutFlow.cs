/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.EntityHandler.Filter
{
    public class OutFlow
    {
        public int Count { get; set; }
        public int PageCount { get; set; }
        public IList<MtdStore> MtdStores { get; set; }

    }
}
