using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class StyleScheduleColor
    {
        public uint Seq { get; set; }
        public uint WFId { get; set; }
        public uint WFDEPInstId { get; set; }
        public uint Depid { get; set; }


        public DateTime ReceviedDateTime { get; set; }
        public uint StyleId { get; set; }
        public string StyleNo { get; set; }
        public string StyleDesc { get; set; }
        public string Ref01 { get; set; }
        public uint ScheduleId { get; set; }
        public string ScheduleNo { get; set; }
        public string ScheduleDesc { get; set; }
        public string Zfeature { get; set; }
        public string PONo { get; set; }
        public uint ColorId { get; set; }
        public string ColorNo { get; set; }
        public string ColorDesc { get; set; }
        public uint SizeId { get; set; }
        public string SizeNo { get; set; }
        public string SizeDesc { get; set; }
        public uint l3id { get; set; }
        public string BagBarcode { get; set; }
        public int SMode { get; set; }
        public int TxnMode { get; set; }

        public int Quantity { get; set; }

        public string Pattern { get; set; }
        public string LotNo { get; set; }
        public int Qty01 { get; set; }
        public int Qty02 { get; set; }
        public int Qty03 { get; set; }
        public int Qty01NS { get; set; }
        public int Qty02NS { get; set; }
        public int Qty03NS { get; set; }

        public DateTime DeliveryDate { get; set; }
        public string[] Responce { get; set; }
        public bool RetiriveSuccessfull { get; set; }
    }
}