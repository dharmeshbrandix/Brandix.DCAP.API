using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class GoodControlDetails
    {
        public uint Seq { get; set; }
        public string ControlId { get; set; }
        public uint ControlType { get; set; }
        public int WFId { get; set; }
        public int Depid { get; set; }
        public int Return { get; set; }
        public DateTime TxnDateTime { get; set; }
        public decimal? Qty01 { get; set; }
        public decimal? Qty02 { get; set; }
        public decimal? Qty03 { get; set; }
        public string JobNo { get; set; }
        public int? OperationCode { get; set; }
        public string EnteredBy { get; set; }
        public int? TxnStatus { get; set; }
        public int? ErrorCode { get; set; }
        public string Remark { get; set; }
        public string Approver { get; set; }
        public string LocCodeFrom { get; set; }
        public string LocCode { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string WatcherEmail { get; set; }
        public string VehicleNo { get; set; }
        public int ApprovalStatus { get; set; }   
        public DateTime? ApprovedDateTime { get; set; }
        public string SecurityPassedBy { get; set; }
        public DateTime? SecurityPassedDateTime { get; set; }
        public string SecurityReceivedBy { get; set; }
        public DateTime? SecurityReceivedDateTime { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedDateTime { get; set; }
        public int InvoiceStatus { get; set; }
        public string InvoiceNumber { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
