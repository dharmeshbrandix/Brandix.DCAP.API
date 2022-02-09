using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class TravelStatus
    {
        public uint WfdepinstId { get; set; }
        public uint Seq { get; set; }
        public uint L1id { get; set; }
        public uint? L2id { get; set; }
        public uint? L3id { get; set; }
        public uint? L4id { get; set; }
        public uint? L5id { get; set; }
        public string BarCodeNo { get; set; }
        public uint Wfid { get; set; }
        public uint Depid { get; set; }
        public uint TeamId { get; set; }
        public uint Dclid { get; set; }
        public uint Dcmid { get; set; }
        public DateTime TxnDateTime { get; set; }
        public int? HourNo { get; set; }
        public int? TxnMode { get; set; }
        public uint? PlussMinus { get; set; }
        public uint? Rrid { get; set; }
        public uint? DopsId { get; set; }        
        public decimal? Qty01 { get; set; }
        public decimal? Qty02 { get; set; }
        public decimal? Qty03 { get; set; }
        public decimal? Qty01Ns { get; set; }
        public decimal? Qty02Ns { get; set; }
        public decimal? Qty03Ns { get; set; }
        public string JobNo { get; set; }
        public int? OperationCode { get; set; }
        public string WorkCenter { get; set; }
        public string EnteredBy { get; set; }
        public int? AppStatus { get; set; }
        public string AppBy { get; set; }
        public DateTime? AppTime { get; set; }
        public int? UploadStatus { get; set; }
        public string UploadBy { get; set; }
        public DateTime? UploadTime { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public int? InstanceNo { get; set; }
        public int? HasError { get; set; }
        public int? IsSucess { get; set; }
        public int? RecStatus { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public string Detxncol { get; set; }

        public Dcl Dcl { get; set; }
        public Dcm Dcm { get; set; }
        public Dep Dep { get; set; }
        public Team Team { get; set; }
        public Wf Wf { get; set; }
        public uint DetxnKey { get; set; }
    }
}
