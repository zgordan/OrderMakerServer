/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Razor.TagHelpers;
using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;


namespace Mtd.OrderMaker.Server.Models.Store
{    
    public class Warehouse
    {        
        public MtdStore Store { get; set; }
        public IList<MtdFormPart> Parts { get; set; }
        public IList<MtdFormPartField> Fields { get; set; }
        public IList<MtdStoreStack> Stack { get; set; }
        public bool SetDate { get; set; }        
    }
}
