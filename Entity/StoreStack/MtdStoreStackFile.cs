/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/


using System.Configuration;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class MtdStoreStackFile
    {
        public long Id { get; set; }
        public byte[] Register { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }

        public virtual MtdStoreStack IdNavigation { get; set; }
    }
}
