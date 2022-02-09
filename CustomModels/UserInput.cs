using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class UserInput
    {
        public uint BagSeq { get; set; }
        public int? BagTxnMode { get; set; }
        public uint Depid { get; set; }
        public uint WFID { get; set; }
        public uint WfdepinstId { get; set; }
        public uint StyleId { get; set; }
        public uint ScheduleId { get; set; }
        public uint ColorId { get; set; }
        public uint SizeId { get; set; }
        public uint DCLId { get; set; }
        public uint DCMId { get; set; }
        public uint TeamId { get; set; }
        public uint L5MOID { get; set; }
        public int ExOpCode { get; set; }
        public int OperationCode { get; set; }
        public int OperationCode2 { get; set; }
        public int HourNo { get; set; }
        public int TxnMode { get; set; }
        public int SMode { get; set; }
         public DateTime TxnDate { get; set; }
        public int PlussMinus { get; set; }
        public string  RRId { get; set; }
        public string  DOpsId { get; set; }
        
        public int EnteredQtyGd { get; set; }
        public int EnteredQtyScrap { get; set; }
        public int EnteredQtyRw { get; set; }

        public int QtytoSaveGd { get; set; }
        public int QtytoSaveScrap {get; set; }
        public int QtytoSaveRw { get; set; }

        public string StyleNo { get; set; }
        public string StyleDesc { get; set; }
        public string Ref01 { get; set; }
        public string ScheduleNo { get; set; }
        public string ScheduleDesc { get; set; }
        public string Zfeature { get; set; }
        public string PONo { get; set; }
        public string ColorNo { get; set; }
        public uint ColorIdUI { get; set; }
        public string ColorDesc { get; set; }
        public string SizeNo { get; set; }
        public string SizeDesc { get; set; }
        public string L5MONo { get; set; }
        public string GroupCode { get; set; }
        public string FacCode { get; set; }
        public string Sbucode { get; set; }
        public string WorkCenter { get; set; }
        public string[] Responce { get; set; }
        public bool SaveSuccessfull { get; set; }
        public string JobNo { get; set; }

        public int RRType { get; set; }
        public string RRName { get; set; }

        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public string AppBy { get; set; }
        public string ErrorDescription { get; set; }

        public int AppStatus { get; set; }
        public int ErrorCode { get; set; }

        public int HasError { get; set; }
        public int IsSucess { get; set; }

        public int RecStatus { get; set; }
        public bool HasAccess { get; set; }
        public string AccessDesc { get; set; }
        public string Remark { get; set; }
    
        public string Barcode { get; set; }

        public string BagBarCodeNo { get; set; }
        public string RefBagBarCodeNo { get; set; }
        public int CounterNumber { get; set; }
        public int ScanType { get; set; }

        public int BagSize { get; set; }

        public int BagStatus { get; set; }

        //public int EnteredQty2 { get; set; } 
        public string ScrapReason { get; set; }
        //public bool SaveSuccessfull { get; set; }

        public int PrevHrGood { get; set; }
        public int PrevHrScrap { get; set; }
        public int PrevHrRework { get; set; }
        public int CurHrGood { get; set; }
        public int CurHrScrap { get; set; }
        public int CurHrRework { get; set; }
        public int TotGood { get; set; }

         public Guid guid { get; set; }

         public int ScanCount { get; set; }

        public int Offline { get; set; }
        public uint DetxnKey { get; set; }

        public Boolean NewCounter { get; set; }
        public Boolean IsUpdateAvilable { get; set; }
        public int CounterId { get; set; }
        public int ProdScanType { get; set; }

        public int seqIndex { get; set; }
    }
}