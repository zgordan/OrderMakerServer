using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public class MtdFilterRelated
    {
        public int Id { get; set; }
        public string FormId { get; set; }
        public int DocBasedNumber { get; set; }

        public virtual MtdFilter IdNavigation { get; set; }
    }
}
