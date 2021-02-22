using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public class MtdStoreTask
    {


        public string Id { get; set; }
        public string MtdStoreId { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public string Initiator { get; set; }
        public string InitNote { get; set; }
        public DateTime InitTimeCr { get; set; }
        public string Executor { get; set; }
        public string ExecNote { get; set; }
        public DateTime ExecTimeCr { get; set; }
        public int Complete { get; set; }
        public sbyte PrivateTask { get; set; }
        public DateTime LastEventTime { get; set; }

        public virtual MtdStore MtdStore { get; set; }

    }
}
