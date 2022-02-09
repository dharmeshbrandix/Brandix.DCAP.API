using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class GroupBarcodeMapping
    {
        public int Seq { get; set; }
        public int WFDEPInstId { get; set; }
        public string MotherBarcode { get; set; }
        public int MotherTxnMode { get; set; }
        public string ChildBarcode { get; set; }
        public int ChildTxnMode { get; set; }
        public decimal? Qty01 { get; set; }
        public decimal? Qty02 { get; set; }
        public decimal? Qty03 { get; set; }
        public decimal? Qty01NS { get; set; }
        public decimal? Qty02NS { get; set; }
        public decimal? Qty03NS { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

    }
}
