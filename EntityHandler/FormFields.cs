using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.EntityHandler
{
    public class FormFields
    {
        public MtdForm MtdForm { get; set; }
        public List<MtdFormPartField> Fields { get; set; }
    }
}
