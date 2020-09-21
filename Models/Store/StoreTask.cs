using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Models.Store
{
    public class StoreTask
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public bool PrivateTask { get; set; }

        public string Initiator { get; set; }
        public string InitNote { get; set; }
        public DateTime InitTimeCr { get; set; }

        public string Executor { get; set; }
        public string ExecNote { get; set; }
        public DateTime ExecTimeCr { get; set; }

        public bool TaskComplete { get; set; }
        public bool TaskRejected { get; set; }
        public bool ButtonClose { get; set; }
        public bool ButtonDelete { get; set; }

    }
}
