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

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdApproval
    {
        public MtdApproval()
        {
            MtdApprovalStage = new HashSet<MtdApprovalStage>();            
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MtdForm { get; set; }

        public byte[] ImgStart { get; set; }
        public byte[] ImgIteraction { get; set; }
        public byte[] ImgWaiting{ get; set; }
        public byte[] ImgApproved { get; set; }
        public byte[] ImgRejected { get; set; }
        public byte[] ImgRequired { get; set; }  

        public virtual MtdForm MtdFormNavigation { get; set; }
        public virtual ICollection<MtdApprovalStage> MtdApprovalStage { get; set; }        
    }
}
