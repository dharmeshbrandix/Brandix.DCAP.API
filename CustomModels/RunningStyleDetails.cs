using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class RunningStyleDetails
    {
        public int? Team_Id  { get; set; }
        public DateTime Txn_DateTime { get; set; }
        public string Key { get; set; }
        public uint L1 { get; set; } 
        public string Style { get; set; } 
        public uint L2 { get; set; }       
        public string Shedule { get; set; }
        public string PO { get; set; }
        public string Zfeature { get; set; }
        public uint L4 { get; set; } 
        public string Color { get; set; }
        public string Size { get; set; }
        
        public string SubinPO { get; set; }
        public string WashDescription { get; set; }
        public int WashProcessID { get; set; }
        public string WashProcess { get; set; }
        public int WashTypeID { get; set; }
        public string WashType { get; set; }
        public string WashName { get; set; }
        public decimal GMTWeight { get; set; }
        public decimal WashDuration { get; set; }
        public decimal UnitPrice { get; set; }
        public string Category { get; set; } 

    }
}