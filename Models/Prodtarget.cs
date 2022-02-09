using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Prodtarget
    {
        public string GroupCode { get; set; }
        public string SBUCode { get; set; }
        public string FacCode { get; set; }
        public uint TeamId { get; set; }
        public DateTime TxnDate { get; set; }
        public uint L1id { get; set; }
        public int? OperationCode { get; set; }
        public uint Depid { get; set; }
        public decimal Qty01 { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
