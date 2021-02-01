using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public class MtdFormula
    {

        public MtdFormula ()
        {
            FormulaBars = new HashSet<MtdFormulaBar>();
        }

        public string Id { get; set; }
        public string FieldId { get; set; }
        public string  Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<MtdFormulaBar> FormulaBars { get; set; }       
        public virtual MtdFormPartField MtdFormPartField { get; set; }


    }
}
