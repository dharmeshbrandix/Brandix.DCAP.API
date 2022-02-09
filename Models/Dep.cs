using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Dep
    {
        public Dep()
        {
            Detxn = new HashSet<Detxn>();
            Wfdep = new HashSet<Wfdep>();
        }

        public uint Depid { get; set; }
        public string Depdesc { get; set; }
        public int StructureNo { get; set; }
        public string Depimage { get; set; }
        public int? OperationCode { get; set; }
        public int ScanOpMode { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public uint CreateBag { get; set; }

        public ICollection<Detxn> Detxn { get; set; }
        public ICollection<Wfdep> Wfdep { get; set; }
    }
}
