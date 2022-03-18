/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/
namespace Mtd.OrderMaker.Server.Models.Store
{
    public class DataContainer
    {
        public Warehouse Owner { get; set; }
        public Warehouse Parent { get; set; }
    }
}
