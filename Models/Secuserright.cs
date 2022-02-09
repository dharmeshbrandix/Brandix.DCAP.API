using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Secuserright
    {
        public string UserId { get; set; }
        public string FunctionId { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedMachine { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedMachine { get; set; }

        public Secfunction Function { get; set; }
        public Secuser User { get; set; }
    }
}
