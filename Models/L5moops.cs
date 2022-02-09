using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L5moops
    {
        public string GroupCode { get; set; }
        public string Sbucode { get; set; }
        public string FacCode { get; set; }
        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public uint L5id { get; set; }
        public uint L5moid { get; set; }
        public int OperationCode { get; set; }
        public string L5mono { get; set; }
        public string WorkCenter { get; set; }
        public decimal OrderQty { get; set; }
        public decimal ReportedQty { get; set; }
        public decimal ScrappedQty { get; set; }
        public DateTime TxnDateTimeMax { get; set; }
        public uint L5moopsStatus { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public L5mo L { get; set; }
    }
}
