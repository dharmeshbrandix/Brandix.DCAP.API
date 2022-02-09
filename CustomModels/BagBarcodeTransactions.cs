using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class BagBarcodeTransactions
    {
        public string Key { get; set; }
        public uint WfdepinstId { get; set; }
        public string Style { get; set; }
        public string Shedule { get; set; }
        public string PO { get; set; }
        public string Z_Feature { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string BagBarCodeNo { get; set; }
        
        public uint WFIdBag { get; set; }
        public string Factory { get; set; }
        public uint DEPIdBag { get; set; }
        public int SeqBag { get; set; }
        public uint L1idBag { get; set; }
        public uint L2idBag { get; set; }
        public uint L3idBag { get; set; }
        public uint L4idBag { get; set; }
        public uint L5idBag { get; set; }

        public int TxnMode { get; set; }

        public int Qty01 { get; set; }
        public int Qty02 { get; set; }
        public int Qty03 { get; set; }

        public int Qty01NS { get; set; }
        public int Qty02NS { get; set; }
        public int Qty03NS { get; set; }

        public int BagStatus { get; set; }
        public string LocationCode { get; set; }
        public string ControlId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string ControlCreatedBy { get; set; }

        public decimal OrderQty { get; set; }
        public decimal OperationQty { get; set; }
    }
}