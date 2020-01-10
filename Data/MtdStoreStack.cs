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

namespace Mtd.OrderMaker.Server.Data
{
    public partial class MtdStoreStack
    {
        public long Id { get; set; }
        public string MtdStore { get; set; }
        public string MtdFormPartField { get; set; }        
        public virtual MtdFormPartField MtdFormPartFieldNavigation { get; set; }
        public virtual MtdStore MtdStoreNavigation { get; set; }
        public virtual MtdStoreLink MtdStoreLink { get; set; }
        public virtual MtdStoreStackDate MtdStoreStackDate { get; set; }
        public virtual MtdStoreStackDecimal MtdStoreStackDecimal { get; set; }
        public virtual MtdStoreStackFile MtdStoreStackFile { get; set; }
        public virtual MtdStoreStackInt MtdStoreStackInt { get; set; }
        public virtual MtdStoreStackText MtdStoreStackText { get; set; }
    }
}
