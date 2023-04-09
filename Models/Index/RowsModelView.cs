/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Models.Index
{
    public class RowsModelView
    {
        public string FormId { get; set; }
        public string SearchNumber { get; set; }
        public int PageCount { get; set; }
        public IList<MtdStore> MtdStores { get; set; }
        public IList<MtdFormPartField> MtdFormPartFields { get; set; }
        public IList<MtdStoreStack> MtdStoreStack { get; set; }
        public bool WaitList { get; set; }
        public bool ShowNumber { get; set; }
        public bool ShowDate { get; set; }
        public List<ApprovalStore> ApprovalStores { get; set; }
        public MtdApproval MtdApproval { get; set; }
        public string StoreIds { get; set; }

        public string SearchText { get; set; }
        public int Pending { get; set; }
        public bool IsCreator { get; set; }
        public int PageSize { get; set; }
        public int PageCurrent { get; set; }

    }
}
