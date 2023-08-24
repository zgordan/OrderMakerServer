using System;

namespace Mtd.Cpq.Manager.Areas.Identity.Data
{
    public class MtdCpqLogAction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime ActionTime { get; set; }
        public int ActionType { get; set; }
        public string DocumentId { get; set; }
    }
}
