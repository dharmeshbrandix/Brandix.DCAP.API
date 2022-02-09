using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Dcl
    {
        public Dcl()
        {
            Detxn = new HashSet<Detxn>();
        }

        public uint Dclid { get; set; }
        public string Dclname { get; set; }
        public uint StructureNo { get; set; }
        public int LevelHierarchy { get; set; }
        public int? IsHide { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public Dclh StructureNoNavigation { get; set; }
        public ICollection<Detxn> Detxn { get; set; }
    }
}
