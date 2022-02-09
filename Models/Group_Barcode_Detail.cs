using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Group_Barcode_Detail
    {
        public uint WfdepinstId { get; set; }
        public uint Seq { get; set; }
        public uint L1id { get; set; }
        public uint? L2id { get; set; }
        public uint? L3id { get; set; }
        public uint? L4id { get; set; }
        public uint? L5id { get; set; }
        public int? L5moid { get; set; }
        public string L5mono { get; set; }
        public string BarCodeNo { get; set; }
        public int? TxnMode { get; set; }
        public string AlterBarcode { get; set; }
        public int? AlterTxnMode { get; set; }
        public uint Wfid { get; set; }
        public uint Depid { get; set; }
        public uint TeamId { get; set; }
        public uint Dclid { get; set; }
        public uint Dcmid { get; set; }
        public DateTime? TxnDateTime { get; set; }      
        public decimal? Qty01 { get; set; }
        public decimal? Qty02 { get; set; }
        public decimal? Qty03 { get; set; }
        public decimal? Qty01Ns { get; set; }
        public decimal? Qty02Ns { get; set; }
        public decimal? Qty03Ns { get; set; }
        public string EnteredBy { get; set; }
        public int? RecStatus { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
        public int DetxnKey { get; set; }
    }
}
