using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L3
    {
        public L3()
        {
            L4 = new HashSet<L4>();
        }

        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public string L3no { get; set; }
        public string L3desc { get; set; }
        public decimal QtyMax { get; set; }
        public uint L3status { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public L2 L { get; set; }
        public ICollection<L4> L4 { get; set; }
    }
}
