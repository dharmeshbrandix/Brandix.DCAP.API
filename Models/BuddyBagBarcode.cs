using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class BuddyBagBarcode
    {
        public uint Seq { get; set; }
        public uint WFId { get; set; }
        public uint DEPId { get; set; }
        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public uint L5id { get; set; }
        public string RefBarcode { get; set; }
        public string BuddyBagBarcodeNo { get; set; }
        public int TxnMode { get; set; }

        public int Qty01 { get; set; }
        public int Qty02 { get; set; }
        public int Qty03 { get; set; }
        public int Qty01NS { get; set; }
        public int Qty02NS { get; set; }
        public int Qty03NS { get; set; }

        public int OperationCode { get; set; }
        public string WorkCenter { get; set; }
        public int RecStatus { get; set; }
        public int TxnStatus { get; set; }

        
        public int JobQty { get; set; }
        public int Weight { get; set; }
        public string TrollyNo { get; set; }
        public DateTime AllocationDate { get; set; }
        public int TravelStatus { get; set; }
        public string FacCode { get; set; }
        public string EPF { get; set; }
        public string Remarks { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
