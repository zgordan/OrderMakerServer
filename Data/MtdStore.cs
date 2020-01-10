/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Data
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
    }
}
