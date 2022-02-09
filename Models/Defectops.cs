using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Defectops
    {
        public int DopsId { get; set; }
        public string DopsCatId { get; set; }
        public string DopsScode { get; set; }
        public string DopsName { get; set; }
        public string DopsDesc { get; set; }
        public string DopsNameS { get; set; }
        public string DopsDescS { get; set; }
        public string DopsNameT { get; set; }
        public string DopsDescT { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
