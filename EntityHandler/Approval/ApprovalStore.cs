namespace Mtd.OrderMaker.Server.EntityHandler.Approval
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
        public string OwnerId { get; set; }
    }
}
