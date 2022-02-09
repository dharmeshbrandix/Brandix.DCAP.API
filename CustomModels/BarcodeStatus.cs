using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class BarcodeStatus
    {
        public DateTime Transaction_Date_and_Time  { get; set; }
        //public int? Operation { get; set; }
        public string Department { get; set; }
        public string BarcodeNo { get; set; }
        public string Style { get; set; }
        public string Schedule_ID { get; set; }
        public string Purchase_Order { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal? Manufacturing_Qty { get; set; }
        public decimal? Qty_Report { get; set; }
        public decimal? Qty_Scrap { get; set; }
        public string Shade_Lot { get; set; }
        public string Shrinkage { get; set; }
        public string BarCodeNo { get; set; }
        public string TravelBarCodeNo { get; set; }
    }
}