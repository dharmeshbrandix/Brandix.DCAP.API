using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Rejectreason
    {
        public int Rrid { get; set; }
        public int? Rrtype { get; set; }
        public string RrcatId { get; set; }
        public int DopsId { get; set; }
        public string Scode { get; set; }
        public string Rrname { get; set; }
        public string Rrdesc { get; set; }
        public string RrnameSin { get; set; }
        public string RrdescSin { get; set; }
        public string RrnameTem { get; set; }
        public string RrdescTem { get; set; }
        public int RejectType { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
