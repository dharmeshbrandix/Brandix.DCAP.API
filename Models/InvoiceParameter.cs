using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class InvoiceParameter
    {
        public int InvoiceKey { get; set; }
        public string NextInvoiceNo { get; set; }
        public decimal VAT { get; set; }
        public decimal NBT { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime EffectiveDateFrom { get; set; }
        public DateTime EffectiveDateTo { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public int RecStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
