using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public class MtdFormulaBar
    {
        public string Id { get; set; }
        public string FormulaId { get; set; }
        public string FieldId { get; set; }
        public int Sequence { get; set; }
        public int Operation { get; set; }

        public virtual MtdFormula MtdFormula { get; set; }
        public virtual MtdFormPartField MtdFormPartField { get; set; }
    }
}
