/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdStore
    {
        public MtdStore()
        {
            InverseParentNavigation = new HashSet<MtdStore>();
            MtdLogDocument = new HashSet<MtdLogDocument>();
            MtdLogApproval = new HashSet<MtdLogApproval>();
            MtdStoreLink = new HashSet<MtdStoreLink>();
            MtdStoreStack = new HashSet<MtdStoreStack>();
            MtdStoreActitvites = new HashSet<MtdStoreActivity>();
            MtdStoreTasks = new HashSet<MtdStoreTask>();
        }

        public string Id { get; set; }
        public int Sequence { get; set; }
        public sbyte Active { get; set; }
        public string MtdForm { get; set; }
        public DateTime Timecr { get; set; }
        public string Parent { get; set; }

        public virtual MtdForm MtdFormNavigation { get; set; }
        public virtual MtdStore ParentNavigation { get; set; }
        public virtual MtdStoreApproval MtdStoreApproval { get; set; }      
        public virtual MtdStoreOwner MtdStoreOwner { get; set; }
        public virtual ICollection<MtdStore> InverseParentNavigation { get; set; }
        public virtual ICollection<MtdLogDocument> MtdLogDocument { get; set; }
        public virtual ICollection<MtdLogApproval> MtdLogApproval { get; set; }
        public virtual ICollection<MtdStoreLink> MtdStoreLink { get; set; }
        public virtual ICollection<MtdStoreStack> MtdStoreStack { get; set; }
        public virtual ICollection<MtdStoreActivity> MtdStoreActitvites { get; set; }
        public virtual ICollection<MtdStoreTask> MtdStoreTasks { get; set; }
    }
}
