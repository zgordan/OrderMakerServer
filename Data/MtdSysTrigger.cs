using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Data
{
    public class MtdSysTrigger
    {
        public MtdSysTrigger()
        {
            MtdFormPartField = new HashSet<MtdFormPartField>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }

        public virtual ICollection<MtdFormPartField> MtdFormPartField { get; set; }
    }
}
