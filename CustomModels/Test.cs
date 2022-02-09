using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class Test
    {
        public uint Team_Id { get; set; }
        public string Team_Name { get; set; }
        public uint L1D { get; set; }
        public string Style { get; set; }
        public uint? L4D { get; set; }
        public string Color { get; set; }
        public int? Operation_Code { get; set; }
        public decimal? Manufacturing_Qty { get; set; }
    }
}