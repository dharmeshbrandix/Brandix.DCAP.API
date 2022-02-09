using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class HourlyProductionStatus
    {
        public DateTime TxnDateTime { get; set; }
        public int? Department_Id  { get; set; }
        public string Department_Name  { get; set; }
        public int? Team_Id  { get; set; }
        public string Team_Name  { get; set; }
        public decimal? Team_Target  { get; set; }
        public Dictionary <string,decimal> Qty { get; set; }
        
    }
}