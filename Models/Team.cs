using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Team
    {
        public Team()
        {
            Detxn = new HashSet<Detxn>();
            TRelatednodes = new HashSet<TRelatednodes>();
            Teamopp = new HashSet<Teamopp>();
            Wfdep = new HashSet<Wfdep>();
        }

        public uint TeamId { get; set; }
        public string GroupCode { get; set; }
        public string Sbucode { get; set; }
        public string FacCode { get; set; }
        public string LocCode { get; set; }
        public string DeptCode { get; set; }
        public string TeamCode { get; set; }
        public string TeamName { get; set; }
        public string Erpcode01 { get; set; }
        public string Erpcode02 { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public ICollection<Detxn> Detxn { get; set; }
        public ICollection<TRelatednodes> TRelatednodes { get; set; }
        public ICollection<Teamopp> Teamopp { get; set; }
        public ICollection<Wfdep> Wfdep { get; set; }
    }
}
