using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public class MtdRegister
    {
        public MtdRegister()
        {
            MtdRegisterFields = new HashSet<MtdRegisterField>();         
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public sbyte ParentLimit { get; set; }

        public virtual ICollection<MtdRegisterField> MtdRegisterFields { get; set; }
    }
}
