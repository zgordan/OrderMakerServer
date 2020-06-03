using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Models.Index
{
    public class ColumnItem
    {
        public string PartId { get; set; }
        public string PartName { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public bool IsChecked { get; set; }
        public int Sequence { get; set; }
    }
}
