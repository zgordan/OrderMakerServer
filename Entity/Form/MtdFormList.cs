/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/


namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdFormList
    {
        public string Id { get; set; }
        public string MtdForm { get; set; }

        public virtual MtdFormPartField IdNavigation { get; set; }
        public virtual MtdForm MtdFormNavigation { get; set; }
    }
}
