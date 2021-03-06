using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Prodhour
    {
        public uint HourNo { get; set; }
        public uint TeamId { get; set; }
        public uint ShiftId { get; set; }
        public uint SeqId { get; set; }
        public string HourName { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public uint EffMinuttes { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
