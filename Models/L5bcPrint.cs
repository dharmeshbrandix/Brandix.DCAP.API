using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class L5bcPrint
    {
        public int Id { get; set; }
        public int L1id { get; set; }
        public string L1desc01 { get; set; }
        public int L2id { get; set; }
        public string L2desc { get; set; }
        public int? L3id { get; set; }
        public string L3no { get; set; }
        public int? L4id { get; set; }
        public string L4no { get; set; }
        public int L5id { get; set; }
        public string L5desc { get; set; }
        public int BcStart { get; set; }
        public int BcCount { get; set; }
        public int? IsPrinted { get; set; }
        public int? IsDuplicateBc { get; set; }
        public int? TeamId { get; set; }
        public string Pattern { get; set; }
        public string LotNo { get; set; }
        public int? RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
