using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Wfdeplink
    {
        public uint WfdepinstId { get; set; }
        public uint WfdepidLink { get; set; }
        public uint LinkType { get; set; }
        public uint RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public Wfdep Wfdepinst { get; set; }
    }
}
