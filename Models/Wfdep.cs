using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Wfdep
    {
        public Wfdep()
        {
            Dedepinst = new HashSet<Dedepinst>();
            Wfdeplink = new HashSet<Wfdeplink>();
            Wfdepmulqty = new HashSet<Wfdepmulqty>();
        }

        public uint WfdepinstId { get; set; }
        public int Wfid { get; set; }
        public uint Depid { get; set; }
        public uint TeamId { get; set; }
        public uint L1id { get; set; }
        public uint Dclid { get; set; }
        public uint Dcmid { get; set; }
        public int? InheritDepid { get; set; }
        public string Sname { get; set; }
        public uint NoOfMulQty { get; set; }
        public uint LimtWithPredecessor { get; set; }
        public int? ExOpCode { get; set; }
        public int? PredDepid { get; set; }
        public int LimitWithPredDclid { get; set; }
        public int? LimitWithPredScrapDclid { get; set; }
        public int? LimitWithWf { get; set; }
        public int LimitWithLevel { get; set; }
        public int LimitDclid { get; set; }
        public uint Bqsplit { get; set; }
        public int SplitDclid { get; set; }
        public int? OperationCode { get; set; }
        public int ScanOpMode { get; set; }
        public int Bccheck { get; set; }
        public string WorkCenter { get; set; }
        public uint Wfdepstatus { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public int ScanCounter { get; set; }
        public int POCounterEnable { get; set; }
        public int ValidateSheduleChange { get; set; }
        public int DataCaptureMode { get; set; }
        public int POCounterNumber { get; set; }
        public int OppValidationQty { get; set; }
        public int RejectReasonSelectMode { get; set; }
        public int BagDepId { get; set; }
        
        public int NextSeqNo { get; set; }
        public int ReceiveEnable {get;set;}
        public int AddNewBag{get;set;}

        public Dep Dep { get; set; }
        public Team Team { get; set; }
        public ICollection<Dedepinst> Dedepinst { get; set; }
        public ICollection<Wfdeplink> Wfdeplink { get; set; }
        public ICollection<Wfdepmulqty> Wfdepmulqty { get; set; }
    }
}
