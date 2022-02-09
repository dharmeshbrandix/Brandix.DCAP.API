using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class WIPReportByTeam
    {
        public string Style { get; set; }
        public string Shedule { get; set; }
        public string PO { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal? Order_Qty { get; set; }
        public Dictionary<string,decimal> OperationQty { get; set; }
        public Dictionary<string,decimal> WIP { get; set; }
    }
}
