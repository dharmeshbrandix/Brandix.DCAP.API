using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Wf
    {
        public Wf()
        {
            Detxn = new HashSet<Detxn>();
        }

        public uint Wfid { get; set; }
        public uint WfidRef { get; set; }
        public string Wfdesc { get; set; }
        public string GroupCode { get; set; }
        public string Sbucode { get; set; }
        public string FacCode { get; set; }
        public int DclstructNo { get; set; }
        public uint L1id { get; set; }
        public uint Wsstatus { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public Factory Factory { get; set; }
        public ICollection<Detxn> Detxn { get; set; }
    }
}
