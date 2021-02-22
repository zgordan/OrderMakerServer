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
    public partial class MtdSysTerm
    {
        public MtdSysTerm()
        {
            MtdFilterField = new HashSet<MtdFilterField>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Sign { get; set; }

        public virtual ICollection<MtdFilterField> MtdFilterField { get; set; }
    }
}
