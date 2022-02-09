using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class SoberSheet
    {
        public string StyleNo { get; set; }       
        public string ScheduleNo { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
        public string Pattern { get; set; }
        public string ShadeLot { get; set; }
        public int Qty { get; set; } 

         public int L1Id { get; set; }
public int L2Id { get; set; }
public int L3Id { get; set; }
public int L4Id { get; set; }
public int L5Id { get; set; }
    }
}