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
    public partial class MtdApprovalResolution
    {
 
        public string Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int Sequence { get; set; }
        public string Color { get; set; }
        public int MtdApprovalStageId { get; set; }
        public byte[] ImgData { get; set; }
        public string ImgType { get; set; }

        public virtual MtdApprovalStage MtdApprovalStage { get; set; }
    }
}
