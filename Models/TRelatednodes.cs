using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class TRelatednodes
    {
        public uint WfdepidLink { get; set; }
        public uint Depid { get; set; }
        public string Depdesc { get; set; }
        public string TeamName { get; set; }
        public uint TeamId { get; set; }

        public Team Team { get; set; }
    }
}
