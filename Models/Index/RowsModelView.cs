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

using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Approval;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Models.Index
{
    public class RowsModelView
    {
        public string IdForm { get; set; }
        public string SearchNumber { get; set; }
        public int PageCount { get; set; }
        public IList<MtdStore> MtdStores { get; set; }
        public IList<MtdFormPartField> MtdFormPartFields { get; set; }
        public IList<MtdStoreStack> MtdStoreStack { get; set; }
        public bool WaitList { get; set; }
        public bool ShowNumber { get; set; }
        public bool ShowDate { get; set; }
        public List<ApprovalStore> ApprovalStores { get; set; }
        public bool IsAppromalForm { get; set; }
    }
}
