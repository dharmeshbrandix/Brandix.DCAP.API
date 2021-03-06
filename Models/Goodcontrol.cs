using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class GoodControl
    {
        public uint Seq { get; set; }
        public string ControlId { get; set; }
        public uint ControlType { get; set; }
        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public uint L5id { get; set; }
        public string BarCodeNo { get; set; }
        public int TxnMode { get; set; }
        public DateTime TxnDateTime { get; set; }
        public decimal? Qty01 { get; set; }
        public decimal? Qty02 { get; set; }
        public decimal? Qty03 { get; set; }

        public int TxnStatus { get; set; }
        public string Remark { get; set; }
        public int? IsSucess { get; set; }
        public string WarLocCode { get; set; }
        public int? RecStatus { get; set; }
        public int Return { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public DateTime RecivedDateTime { get; set; }
    }
}
