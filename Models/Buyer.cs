using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Buyer
    {
        public Buyer()
        {
            Buyerdiv = new HashSet<Buyerdiv>();
        }

        public uint BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string Name { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public ICollection<Buyerdiv> Buyerdiv { get; set; }
    }
}
