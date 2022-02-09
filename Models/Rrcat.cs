using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Rrcat
    {
        public string RrcatId { get; set; }
        public int Rrtype { get; set; }
        public string RrcatName { get; set; }
        public string RrcatNameSin { get; set; }
        public string RrcatNameTem { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
