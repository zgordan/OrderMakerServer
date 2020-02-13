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

using System.Collections.Generic;

namespace Mtd.OrderMaker.Server.Data
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

        public virtual MtdFormPart MtdFormPartNavigation { get; set; }
        public virtual MtdSysType MtdSysTypeNavigation { get; set; }
        public virtual MtdFormList MtdFormList { get; set; }
        public virtual ICollection<MtdFilterColumn> MtdFilterColumn { get; set; }
        public virtual ICollection<MtdFilterField> MtdFilterField { get; set; }
        public virtual ICollection<MtdStoreStack> MtdStoreStack { get; set; }
    }
}
