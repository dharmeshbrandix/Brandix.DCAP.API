using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L5mo
    {
        public L5mo()
        {
            L5moops = new HashSet<L5moops>();
        }

        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public uint L5id { get; set; }
        public uint L5moid { get; set; }
        public string L5mono { get; set; }
        public decimal QtyMax { get; set; }
        public uint L5mostatus { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public L5 L { get; set; }
        public ICollection<L5moops> L5moops { get; set; }
    }
}
