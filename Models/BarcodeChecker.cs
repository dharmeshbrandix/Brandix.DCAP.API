using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class BarcodeChecker
    {
        public string BarCodeNo { get; set; }
        public string TravelBarCodeNo { get; set; }
        public int? OperationCode { get; set; }
        public uint DetxnKey { get; set; }
        public decimal? Qty01 { get; set; }
    }
}
