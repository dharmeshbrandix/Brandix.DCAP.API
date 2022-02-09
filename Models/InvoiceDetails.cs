using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class InvoiceDetails
    {
        public int InvoiceDetailKey { get; set; }
        public int InvoiceSeq { get; set; }
        public string InvoiceNo { get; set; }
        public string ControlId { get; set; }
        public int ControlType { get; set; }
        public int Seq { get; set; }
        public uint WFId { get; set; }
        public uint L1Id { get; set; }
        public uint L2Id { get; set; }
        public uint L3Id { get; set; }
        public uint L4Id { get; set; }
        public uint L5Id { get; set; }
        public DateTime TxnDateTime { get; set; }
        public string BarcodeNo { get; set; }
        public decimal Qty01 { get; set; }
        public decimal Qty02 { get; set; }
        public decimal Qty03 { get; set; }
        public decimal Price { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
