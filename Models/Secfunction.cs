using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Secfunction
    {
        public Secfunction()
        {
            Secuserright = new HashSet<Secuserright>();
        }

        public string FunctionId { get; set; }
        public string FuncName { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedMachine { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedMachine { get; set; }
        public string LocWiseAccess { get; set; }

        public ICollection<Secuserright> Secuserright { get; set; }
    }
}
