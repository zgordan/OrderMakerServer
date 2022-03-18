/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/


namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdFilterColumn
    {
        public int Id { get; set; }
        public int MtdFilter { get; set; }
        public string MtdFormPartField { get; set; }
        public int Sequence { get; set; }

        public virtual MtdFilter MtdFilterNavigation { get; set; }
        public virtual MtdFormPartField MtdFormPartFieldNavigation { get; set; }
    }
}
