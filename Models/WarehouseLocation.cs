using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class WarehouseLocation
    {
        public string FacCode { get; set; }
        public string WarCode { get; set; }
        public string WarName { get; set; }
        public string WarLocCode { get; set; }
        public string WarLocName { get; set; }
        public string WarLocAddress { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
