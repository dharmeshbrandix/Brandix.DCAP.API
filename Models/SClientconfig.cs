using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class SClientconfig
    {
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public int? OpCode1 { get; set; }
        public int? OpCode2 { get; set; }
        public string OperationName { get; set; }
        public int? SelectMode { get; set; }
        public int? DataCaptureMode { get; set; }
        public int? LoginMode { get; set; }
        public int? RecStatus { get; set; }
        public string TeamName { get; set; }
        public string FacCode { get; set; }
        public string FacName { get; set; }
        public int? TeamId { get; set; }
        public int? WfdepinstId { get; set; }
        public int? WfId { get; set; }
        public string ClientIP { get; set; }
        public int ScanCounter { get; set; }
        public int POCounterEnable { get; set; }
        public int ValidateSheduleChange { get; set; }
        public int POCounterNumber { get; set; }
        public int OppValidationQty { get; set; }
    }
}
