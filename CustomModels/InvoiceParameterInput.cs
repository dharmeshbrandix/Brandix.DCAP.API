using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class InvoiceParameterInput
    {
        public int Mode { get; set; }
        public int InvoiceKey { get; set; }
        public string NextInvoiceNo { get; set; }
        public decimal VAT { get; set; }
        public decimal NBT { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime EffectiveDateFrom { get; set; }
        public DateTime EffectiveDateTo { get; set; }
        public int RecStatus { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }

        public string[] Responce { get; set; }
        public bool SaveSuccessfull { get; set; }
    }
}
