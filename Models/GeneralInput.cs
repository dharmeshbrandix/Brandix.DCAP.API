using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class GeneralInput
    {
        public int L1id { get; set; }
        
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public string[] Counts { get; set; }
        public string[] Responce { get; set; }
        public bool SaveSuccessfull { get; set; }
    }
}
