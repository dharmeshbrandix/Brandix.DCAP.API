using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class ProducrionStatus
    {
        public DateTime Date_and_Time  { get; set; }
        public string Style  { get; set; }
        public string Shedule  { get; set; }
        public string Color  { get; set; }
        public string Size  { get; set; }
        public string Team_Name  { get; set; }
        public string Barcode  { get; set; }
        public string Rework_Cat  { get; set; }
        public string Rework_Reason  { get; set; }
        public decimal? Rework_Quantity { get; set; }
        public int? Record_Status { get; set; }

    }
}