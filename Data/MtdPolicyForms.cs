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
using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdPolicyForms
    {
        public int Id { get; set; }
        public string MtdPolicy { get; set; }
        public string MtdForm { get; set; }
        public sbyte Create { get; set; }
        public sbyte EditAll { get; set; }
        public sbyte EditGroup { get; set; }
        public sbyte EditOwn { get; set; }
        public sbyte ViewAll { get; set; }
        public sbyte ViewGroup { get; set; }
        public sbyte ViewOwn { get; set; }
        public sbyte DeleteAll { get; set; }
        public sbyte DeleteGroup { get; set; }
        public sbyte DeleteOwn { get; set; }
        public sbyte ChangeOwner { get; set; }
        public sbyte Reviewer { get; set; }
        public sbyte ChangeDate { get; set; }

        public virtual MtdForm MtdFormNavigation { get; set; }
        public virtual MtdPolicy MtdPolicyNavigation { get; set; }
    }
}
