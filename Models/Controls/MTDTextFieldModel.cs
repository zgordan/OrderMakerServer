using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Models.Controls
{
    public class MTDTextFieldModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }
        public string IconLeading { get; set; }
        public string IconLeadingScript { get; set; }
        public string IconTrailing { get; set; }
        public string IconTrailingString { get; set; }
        public bool Required { get; set; }
        public bool Disabled { get; set; }
        public string HelperText { get; set; }
        public string Type { get; set; }
        public int Counter { get; set; }

    }
}
