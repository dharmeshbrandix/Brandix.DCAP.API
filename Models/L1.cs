using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L1
    {
        public L1()
        {
            L2 = new HashSet<L2>();
        }

        public uint L1id { get; set; }
        public string L1no { get; set; }
        public string L1desc { get; set; }
        public int StructureNo { get; set; }
        //public uint Wfid { get; set; }
        public string Ref01 { get; set; }
        public string Ref02 { get; set; }
        public string Ref03 { get; set; }
        public string Ref04 { get; set; }
        public string Ref05 { get; set; }
        public decimal QtyMax { get; set; }
        public uint L1status { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string L1colVijitha { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public string AchivedComments { get; set; }
        public uint BuyerDivId { get; set; }
        public uint BuyerId { get; set; }

        public ICollection<L2> L2 { get; set; }
    }
}
