using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Dclh
    {
        public Dclh()
        {
            Dcl = new HashSet<Dcl>();
        }

        public uint StructureNo { get; set; }
        public string StructName { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public ICollection<Dcl> Dcl { get; set; }
    }
}
