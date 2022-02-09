using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class BarcodeDetailAsync
    {
        public string Key { get; set; }
        public uint StyleId { get; set; }
        public string StyleNo { get; set; }
        public uint ScheduleId { get; set; }
        public string ScheduleNo { get; set; }
        public string Zfeature { get; set; }
        public string PONo { get; set; }
        public uint ColorId { get; set; }
        public string ColorDesc { get; set; }
        public uint SizeId { get; set; }
        public string SizeDesc { get; set; }
        public string L5mono { get; set; }
        public int? OperationCode { get; set; }
        public int EnteredQtyGd { get; set; }
        public int EnteredQtyScrap { get; set; }
        public int EnteredQtyRw { get; set; }
        public string Barcode { get; set; }
        public string BagBarCodeNo { get; set; }
        public string RefBagBarCodeNo { get; set; }
        public Dictionary <string , decimal> Qty { get; set; }
        
    }
}