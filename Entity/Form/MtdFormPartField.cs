/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdFormPartField
    {
        public MtdFormPartField()
        {
            MtdFilterColumn = new HashSet<MtdFilterColumn>();
            MtdFilterField = new HashSet<MtdFilterField>();
            MtdStoreStack = new HashSet<MtdStoreStack>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public sbyte Required { get; set; }
        public int Sequence { get; set; }
        public sbyte Active { get; set; }
        public sbyte ReadOnly { get; set; }
        public int MtdSysType { get; set; }
        public string MtdFormPart { get; set; }
        public string MtdSysTrigger { get; set; }
        public string DefaultData { get; set; }

        public virtual MtdFormPart MtdFormPartNavigation { get; set; }
        public virtual MtdSysType MtdSysTypeNavigation { get; set; }
        public virtual MtdSysTrigger MtdSysTriggerNavigation { get; set; }
        public virtual MtdFormList MtdFormList { get; set; }
        public virtual ICollection<MtdFilterColumn> MtdFilterColumn { get; set; }
        public virtual ICollection<MtdFilterField> MtdFilterField { get; set; }
        public virtual ICollection<MtdStoreStack> MtdStoreStack { get; set; }
        public virtual MtdRegisterField MtdRegisterField { get; set; }

    }
}
