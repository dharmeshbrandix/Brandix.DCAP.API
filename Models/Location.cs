using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Location
    {
        public Location()
        {
            Department = new HashSet<Department>();
        }

        public string GroupCode { get; set; }
        public string Sbucode { get; set; }
        public string FacCode { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string LocAddress { get; set; }
        public string LocDescription { get; set; }
        public string VATNo { get; set; }
        public string SVATNo { get; set; }
        public string Atten { get; set; }
        public string TelNo { get; set; }
        public string CustomerNo { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public Factory Factory { get; set; }
        public ICollection<Department> Department { get; set; }
    }
}
