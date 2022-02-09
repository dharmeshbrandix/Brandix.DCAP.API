using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Secuserrightdep
    {
        public string UserId { get; set; }
        public string FunctionId { get; set; }
        public string GroupCode { get; set; }
        public string Sbucode { get; set; }
        public string FacCode { get; set; }
        public uint Depid { get; set; }
        public int StructureNo { get; set; }
        public uint RecStatus { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedMachine { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
