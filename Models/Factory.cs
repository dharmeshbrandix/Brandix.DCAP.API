﻿using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Factory
    {
        public Factory()
        {
            Location = new HashSet<Location>();
            Wf = new HashSet<Wf>();
        }

        public string GroupCode { get; set; }
        public string Sbucode { get; set; }
        public string FacCode { get; set; }
        public string FacName { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public ICollection<Location> Location { get; set; }
        public ICollection<Wf> Wf { get; set; }
    }
}
