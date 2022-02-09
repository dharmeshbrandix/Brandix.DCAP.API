using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L2
    {
        public L2()
        {
            L3 = new HashSet<L3>();
        }

        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public string L2no { get; set; }
        public string L2desc { get; set; }
        public uint Wfid { get; set; }
        public decimal QtyMax { get; set; }
        public uint L2status { get; set; }
        public string Ref01 { get; set; }
        public string Ref02 { get; set; }
        public uint RecStatus { get; set; }
        public int NextBcNo { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public DateTime DeliveryDate { get; set; }

        public L1 L1 { get; set; }
        public ICollection<L3> L3 { get; set; }
    }
}
