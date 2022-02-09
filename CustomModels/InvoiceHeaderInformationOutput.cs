using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class InvoiceHeaderInformationOutput
    {
        public string InvoiceNo { get; set; }
        public string BillTo { get; set; }
        public string DispatchTo { get; set; }
        public string BillFrom { get; set; }
        public string VATNoTo { get; set; }
        public string VATNoFrom { get; set; }
        public string SVATNoTo { get; set; }
        public string SVATNoFrom { get; set; }
        public string Atten { get; set; }
        public string TelNo { get; set; }
        public string OrderNo { get; set; }
        public string DeliveryNo { get; set; }
        public string DeliveryDate { get; set; }
        public DateTime? TxnDateTime { get; set; }
        public decimal VAT { get; set; }
        public decimal NBT { get; set; }
        public decimal NBTValue { get; set; }
        public decimal ValueAddedTaxInUSD { get; set; }
        public decimal TotalInvoiceVal { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal VatinLKR { get; set; }
        public decimal VatSuspendinLKR { get; set; }
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
