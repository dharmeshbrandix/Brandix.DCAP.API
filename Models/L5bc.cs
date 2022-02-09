using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L5bc
    {
        public string BarCodeNo { get; set; }
        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public uint L5id { get; set; }
        public string L5desc { get; set; }
        public int? QtyMax { get; set; }
        public int? L5bcstatus { get; set; }
        public int? L5bcisUsed { get; set; }
        public int? IsPrinted { get; set; }
        public int? IsDuplicateBc { get; set; }
        public string Pattern { get; set; }
        public int? TeamId { get; set; }
        public string LotNo { get; set; }
        public int? RecStatus { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
