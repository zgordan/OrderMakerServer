using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler
{
    public class FormFields
    {
        public MtdForm MtdForm { get; set; }
        public List<MtdFormPartField> Fields { get; set; }
    }
}
