using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class TeamCounterOutput
    {
        //Controllers
        public Boolean IsUpdateAvilable { get; set; }
        public Boolean NewCounter { get; set; }
        public Boolean updated { get; set; }

        //Columns
        public int CounterId { get; set; }
        public uint WfdepinstId { get; set; }
        public int CounterType { get; set; }
        public uint L1id { get; set; }
        public uint L2id { get; set; }
        public uint L3id { get; set; }
        public uint L4id { get; set; }
        public uint L5id { get; set; }
        public uint L5MOID { get; set; }
        public string BagBarCodeNo { get; set; }
        public string RefBagBarCodeNo { get; set; }
        public int BagSize { get; set; }

        public int Qty01 { get; set; }
        public int Qty02 { get; set; }
        public int Qty03 { get; set; }
        public int CutQty { get; set; }
        public int BagStatus { get; set; }
        public int RecStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public string[] Response { get; set; }
    }
}
