using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Buyerdiv
    {
        public uint BuyerId { get; set; }
        public uint BuyerDivId { get; set; }
        public string BuyerDivCode { get; set; }
        public string Name { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public Buyer Buyer { get; set; }
    }
}
