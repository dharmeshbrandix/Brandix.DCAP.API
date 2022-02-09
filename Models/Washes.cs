﻿using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Washes
    {
        public int WashCatId { get; set; }
        public int WashType { get; set; }
        public string WashName { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
