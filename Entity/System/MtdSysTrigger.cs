using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Entity
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
