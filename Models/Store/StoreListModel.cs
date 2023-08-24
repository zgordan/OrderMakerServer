using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Models.Store
{
    public class StoreListModel
    {
        public string ListId { get; set; }
        public string ImgSrc { get; set; }
        public MtdForm CurentForm { get; set; }
        public List<MtdForm> AvailableForms { get; set; }
        public List<StoreListFields> Store { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public string PageLine { get; set; }
        public List<string> Columns { get; set; }

        public string ResultTarget { get; set; }
        public string ResultValue { get; set; }
        public string ResultView { get; set; }
        public List<MTDSelectListItem> RelatedForms { get; set; }
    }

    public class StoreListFields
    {
        public string StoreId { get; set; }
        public List<StoreListField> Fields { get; set; }
    }

    public class StoreListField
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string SysType { get; set; }
        public string Name { get; set; }
    }
}
