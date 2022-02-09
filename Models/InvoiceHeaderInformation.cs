using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class InvoiceHeaderInformation
    {
        public string InvoiceNo { get; set; }
        public DateTime? TxnDateTime { get; set; }
        public decimal VAT { get; set; }
        public decimal NBT { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceIncludingVAT { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
