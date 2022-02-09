using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L4
    {
        public L4()
        {
            L5 = new HashSet<L5>();
        }

        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public string L4no { get; set; }
        public string L4desc { get; set; }
        public decimal QtyMax { get; set; }
        public uint L4status { get; set; }
        public int WashCatId { get; set; }
        public int SFCatId { get; set; }
        public string Category { get; set; }
        public string SubinPO { get; set; }
        public decimal GarmentWeight { get; set; }
        public decimal WashDuration { get; set; }
        public decimal UnitPrice { get; set; }
        public string WashDescription { get; set; }
        public string WashType { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public L3 L { get; set; }
        public ICollection<L5> L5 { get; set; }
    }
}
