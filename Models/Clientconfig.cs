using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Clientconfig
    {
        public string ClientId { get; set; }
        public int? WfdepinstId { get; set; }        
        public int? WfId { get; set; }
        public string UserId { get; set; }
        public uint TeamId { get; set; }
        public int? OpCode1 { get; set; }
        public int? OpCode2 { get; set; }
        public string OperationName { get; set; }
        public int SelectMode { get; set; }
        public int DataCaptureMode { get; set; }
        public int LoginMode { get; set; }
        public int TxnMode { get; set; }
        public int RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public string ClientIp { get; set; }
    }
}
