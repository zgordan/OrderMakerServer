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

using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdApprovalStage
    {
        public MtdApprovalStage()
        {
            MtdApprovalResolution = new HashSet<MtdApprovalResolution>();
            MtdApprovalRejection = new HashSet<MtdApprovalRejection>();
            MtdLogApproval = new HashSet<MtdLogApproval>();
            MtdStoreApproval = new HashSet<MtdStoreApproval>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MtdApproval { get; set; }
        public int Stage { get; set; }
        public string UserId { get; set; }
        public string BlockParts { get; set; }

        public virtual MtdApproval MtdApprovalNavigation { get; set; }
        public virtual ICollection<MtdApprovalResolution> MtdApprovalResolution { get; set; }
        public virtual ICollection<MtdApprovalRejection> MtdApprovalRejection { get; set; }
        public virtual ICollection<MtdLogApproval> MtdLogApproval { get; set; }
        public virtual ICollection<MtdStoreApproval> MtdStoreApproval { get; set; }
    }
}
