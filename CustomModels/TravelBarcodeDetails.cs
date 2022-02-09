using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class TravelBarcodeDetailsOut
    {
        public uint Seq { get; set; }
        public uint WFId { get; set; }
        public uint DEPId { get; set; }
        public string Color { get; set; }
        public string TravelBarCodeNo { get; set; }
        public int TxnMode { get; set; }
        public int SplitStatus { get; set; }

        public decimal Qty01 { get; set; }
        public decimal Qty02 { get; set; }
        public decimal Qty03 { get; set; }
        public decimal Qty01NS { get; set; }
        public decimal Qty02NS { get; set; }
        public decimal Qty03NS { get; set; }
        public int JobQty { get; set; }
        public int Weight { get; set; }
        public int OperationCode { get; set; }
        public string WorkCenter { get; set; }
        public int RecStatus { get; set; }
        public string TrollyNo { get; set; }
        public DateTime AllocationDate { get; set; }
        public int Status { get; set; }
        public string TravelStatus { get; set; }
        public string EPF { get; set; }
        public string Remarks { get; set; }
        public string FacCode { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
