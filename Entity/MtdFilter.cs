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

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdFilter
    {
        public MtdFilter()
        {
            MtdFilterColumn = new HashSet<MtdFilterColumn>();
            MtdFilterField = new HashSet<MtdFilterField>();
            MtdFilterScriptApply = new HashSet<MtdFilterScriptApply>();
        }

        public int Id { get; set; }
        public string IdUser { get; set; }
        public string MtdForm { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; }
        public string SearchNumber { get; set; }
        public int Page { get; set; }
        public int WaitList { get; set; }
        public sbyte ShowNumber { get; set; }
        public sbyte ShowDate { get; set; }

        public virtual MtdForm MtdFormNavigation { get; set; }
        public virtual MtdFilterDate MtdFilterDate { get; set; }
        public virtual MtdFilterOwner MtdFilterOwner { get; set; }
        public virtual ICollection<MtdFilterColumn> MtdFilterColumn { get; set; }
        public virtual ICollection<MtdFilterField> MtdFilterField { get; set; }
        public virtual ICollection<MtdFilterScriptApply> MtdFilterScriptApply { get; set; }
    }
}
