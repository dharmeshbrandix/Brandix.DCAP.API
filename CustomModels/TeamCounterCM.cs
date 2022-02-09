using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class TeamCounterCM
    {
        public uint Seq { get; set; }
        public uint WFIdBag { get; set; }
        public string WFIdDesc { get; set; }
        public int CounterId { get; set; }

        public string ControlId { get; set; }
        public string ControlCeatedBy { get; set; }
        public string WarLocCode { get; set; }
        public string TravelBarCodeNo { get; set; }
        public int RRType { get; set; }
        public int RRId { get; set; }
        public string RRName { get; set; }
        public uint L1idBag { get; set; }
        public uint L2idBag { get; set; }
        public uint L3idBag { get; set; }
        public uint L4idBag { get; set; }
        public uint L5idBag { get; set; }
        public uint WFDEPInstId { get; set; }
        public uint StyleId { get; set; }
        public string StyleNo { get; set; }
        public string StyleDesc { get; set; }
        public string Ref01 { get; set; }
        public string Ref02 { get; set; }
        public uint ScheduleId { get; set; }
        public string ScheduleNo { get; set; }
        public string ScheduleDesc { get; set; }
        public string PONo { get; set; }
        public string Zfeature { get; set; }
        public DateTime DeliveryDate { get; set; }
        public uint ColorId { get; set; }
        public string ColorNo { get; set; }
        public string ColorDesc { get; set; }
        public uint SizeId { get; set; }
        public string SizeNo { get; set; }
        public string SizeDesc { get; set; }
        public string BagBarCode { get; set; }
        public int TxnMode { get; set; }
        public int BagSize { get; set; }
        public int CounterNumber { get; set; }
        public int TxnStatus { get; set; }
        public string Location  { get; set; }

        public int Qty01 { get; set; }
        public int Qty02 { get; set; }
        public int Qty03 { get; set; }
        public int Qty01Ns { get; set; }
        public int Qty02Ns { get; set; }
        public int Qty03Ns { get; set; }

        public int JobQty { get; set; }
        public int Weight { get; set; }
        public string TrollyNo { get; set; }
        public string Remarks { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public int CutQuantity { get; set; }
        public int BagStatus { get; set; }
        public string Remark  { get; set; }
    }
}