/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/


using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdForm
    {
        public MtdForm()
        {
            InverseParentNavigation = new HashSet<MtdForm>();
            MtdApproval = new HashSet<MtdApproval>();
            MtdFilter = new HashSet<MtdFilter>();
            MtdFormList = new HashSet<MtdFormList>();
            MtdFormPart = new HashSet<MtdFormPart>();
            MtdPolicyForms = new HashSet<MtdPolicyForms>();
            MtdStore = new HashSet<MtdStore>();
            MtdFilterScript = new HashSet<MtdFilterScript>();
            MtdParentForms = new HashSet<MtdFormRelated>();
            MtdChildForms = new HashSet<MtdFormRelated>();
            MtdFormActivites = new HashSet<MtdFormActivity>();
            MtdEventSubscribes = new HashSet<MtdEventSubscribe>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public sbyte Active { get; set; }
        public string MtdCategory { get; set; }
        public int Sequence { get; set; }
        public string Parent { get; set; }
        public sbyte VisibleNumber { get; set; }
        public sbyte VisibleDate { get; set; }

        public virtual MtdCategoryForm MtdCategoryNavigation { get; set; }
        public virtual MtdForm ParentNavigation { get; set; }
        public virtual MtdFormDesk MtdFormDesk { get; set; }
        public virtual MtdFormHeader MtdFormHeader { get; set; }
        public virtual ICollection<MtdForm> InverseParentNavigation { get; set; }
        public virtual ICollection<MtdApproval> MtdApproval { get; set; }
        public virtual ICollection<MtdFilter> MtdFilter { get; set; }
        public virtual ICollection<MtdFormList> MtdFormList { get; set; }
        public virtual ICollection<MtdFormPart> MtdFormPart { get; set; }
        public virtual ICollection<MtdPolicyForms> MtdPolicyForms { get; set; }
        public virtual ICollection<MtdStore> MtdStore { get; set; }
        public virtual ICollection<MtdFilterScript> MtdFilterScript { get; set; }        
        public virtual ICollection<MtdFormRelated> MtdParentForms { get; set; }
        public virtual ICollection<MtdFormRelated> MtdChildForms { get; set; }
        public virtual ICollection<MtdFormActivity> MtdFormActivites { get; set; }
        public virtual ICollection<MtdEventSubscribe> MtdEventSubscribes { get; set; }
    }
}

