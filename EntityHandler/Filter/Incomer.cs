/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Mtd.OrderMaker.Server.EntityHandler.Filter
{
    public class Incomer
    {     
        public string FormId { get; set; }
        public string SearchText { get; set; }
        public string SearchNumber { get; set; }
        public int PageSize { get;  set; }
        public int Page { get; set; }
        public int WaitList { get; set; }
        public IList<MtdFormPartField> FieldForColumn { get; set; }
        public List<string> FieldIds => FieldForColumn.Select(x => x.Id).ToList();
        public IList<MtdFilterField> FieldForFilter { get; set; }
        
    }
}
