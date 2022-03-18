/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

namespace Mtd.OrderMaker.Server.Models.Index
{
    public class FooterModelView
    {
        public string FormId { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
    }
}
