using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class WIPReportByTeamCapture
    {
        public uint? L1D { get; set; }
        public uint? L2D { get; set; }
        public uint? L3D { get; set; }
        public uint? L4D { get; set; }
        public uint? L5D { get; set; }
        public int? L5MD { get; set; }
        public decimal? Manufacturing_Qty { get; set; }

    }
}
