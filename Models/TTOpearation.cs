using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class TTOpearation
    {
        public uint Seq { get; set; }
        public string BarCodeNo { get; set; }
        public uint WFDEPInstId { get; set; }
        public uint WFId { get; set; }
        public int TxnMode { get; set; }
        public int TeamId { get; set; }
        public DateTime? TxnDateTime { get; set; }

        public int Qty01 { get; set; }
        public int Qty02 { get; set; }
        public int Qty03 { get; set; }
        public int Qty01NS { get; set; }
        public int Qty02NS { get; set; }
        public int Qty03NS { get; set; }
        public int OperationCode { get; set; }
        public int PlussMinus { get; set; }
        public int RecStatus { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
