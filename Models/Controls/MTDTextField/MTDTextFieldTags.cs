﻿using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Models.Controls.MTDTextField
{
    public class MTDTextFieldTags
    {        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool LabelLocalized { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }
        public bool PlaceholderLocalized { get; set; }
        public string IconLeading { get; set; }
        public string IconLeadingScript { get; set; }
        public string IconTrailing { get; set; }
        public string IconTrailingString { get; set; }
        public bool Required { get; set; }
        public bool Disabled { get; set; }
        public string HelperText { get; set; }
        public bool HelperTextLocalizer { get; set; }
        public string HelperError { get; set; }
        public bool HelperErrorLocalizer { get; set; }
        public string Type { get; set; }
        public int MaxLength { get; set; }
        public string Class { get; set; }
        public MTDTextFieldViews MTDTexFieldView { get; set; }

        public int Rows { get; set; }
        public int Cols { get; set; }

        public string Pattern { get; set; }
        public bool MtdInputClicker { get; set; }
        public string MtdDataMessage { get; set; }
    }
}
