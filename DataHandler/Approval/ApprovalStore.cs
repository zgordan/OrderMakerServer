using Mtd.OrderMaker.Server.DataHandler.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.DataHandler.Approval
{
    public enum ApprovalStatus
    {
        Start,
        Iteraction,
        Waiting,
        Approved,
        Rejected,
        Required
    }

    public class ApprovalStore
    {
        public string StoreId { get; set; }
        public ApprovalStatus Status { get; set; }
    }
}
