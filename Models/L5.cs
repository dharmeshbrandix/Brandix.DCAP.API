using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L5
    {
        public L5()
        {
            L5mo = new HashSet<L5mo>();
        }

        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public uint L5id { get; set; }
        public string L5no { get; set; }
        public string L5desc { get; set; }
        public decimal QtyMax { get; set; }
        public uint L5status { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public L4 L { get; set; }
        public ICollection<L5mo> L5mo { get; set; }
    }
}
