using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Models.Controls
{
    public class MTDSListModel      
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string ValueId { get; set; }        
        public Dictionary<string, string> Items { get; set; }
        
    }
}
