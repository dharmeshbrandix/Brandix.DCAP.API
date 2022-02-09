using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class HourlyCounts
    {
        public int PrevHrGood { get; set; }
        public int PrevHrScrap { get; set; }
        public int PrevHrRework { get; set; }
        public int CurHrGood { get; set; }
        public int CurHrScrap { get; set; }
        public int CurHrRework { get; set; }
        public int TotGood { get; set; }
        public int CurHourNo { get; set; }
    }
}