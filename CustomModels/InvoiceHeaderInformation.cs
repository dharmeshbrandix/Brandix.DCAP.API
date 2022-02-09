using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class InvoiceSummary
    {
        public string Division { get; set; }
        public string Customer_Number { get; set; }
        public string Payer { get; set; }
        public string Invoice_Number { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public decimal Invoice_Amount_For { get; set; }
        public decimal VAT { get; set; }
        public decimal NBT { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Dimension_1 { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedMachine { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedMachine { get; set; }
    }
}
