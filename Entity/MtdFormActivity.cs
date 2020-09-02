using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public class MtdFormActivity
    {
        public MtdFormActivity()
        {
            MtdStoreActivites = new HashSet<MtdStoreActivity>();
        }

        public string Id { get; set; }
        public string MtdFomrId { get; set; }
        public byte[] Image { get; set; }
        public string ImageType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual MtdForm MtdForm { get; set; }
        public virtual ICollection<MtdStoreActivity> MtdStoreActivites { get; set; }
    }


}
