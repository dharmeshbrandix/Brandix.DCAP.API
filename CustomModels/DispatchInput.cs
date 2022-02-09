using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class DispatchInput
    {
        public string DispatchBarcode { get; set; }
        public uint Type { get; set; }
        public int Return { get; set; }
        public uint WFIdBag { get; set; }
        public uint DEPIdBag { get; set; }
        public uint SeqBag { get; set; }
        public uint SeqG { get; set; }
        public uint SeqGD { get; set; }
        public uint L1idBag { get; set; }
        public uint L2idBag { get; set; }
        public uint L3idBag { get; set; }
        public uint L4idBag { get; set; }
        public uint L5idBag { get; set; }
        public string Barcode { get; set; }
        public string BagBarcode { get; set; }
        public string BuddyBagBarcode { get; set; }
        public uint Wfid { get; set; }
        public uint departmentTo  { get; set; }
        public DateTime TxnDateandTime { get; set; }
        public uint TxnMode { get; set; }
        public int TxnStatus { get; set; }
        public decimal Qty01 { get; set; }
        public uint OperationCode { get; set; }
        public string EnterdBy { get; set; }
        public string Remark { get; set; }
        public string LocFrom { get; set; }
        public string LocCode { get; set; }
        public string WarLocCode { get; set; }
        public string Approver { get; set; }
        public int ApprovalStatus { get; set; }
        public string receiverName { get; set; }
        public string receiverEmail { get; set; }
        public string watcherEmail { get; set; }
        public string courierNo { get; set; }
        public string gpRemarks { get; set; }
        public string vehicleNo { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public Boolean SaveSuccessful { get; set; }
    }
}