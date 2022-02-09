using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class BFLStockSummary
    {
        public int Week { get; set; }
        public string BuyerDivision { get; set; }
        public string Season { get; set; }
        public string Factory { get; set; }
        public string ProductionWarehouse { get; set; }
        public string  ProductionWarehouseName { get; set; }
        public string Style { get; set; }
        public string Shedule { get; set; }
        public string SubinPO { get; set; }
        public string PO { get; set; }
        public string Zfeature { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public DateTime ActualReciveDate { get; set; }
        public DateTime LastLotReciveDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string BarcodeNo { get; set; }
        public string ControlId { get; set; }
        public DateTime TxnDateTime { get; set; }
        public string WarLocCode { get; set; }
        public string Remarks { get; set; }
        public decimal? Price { get; set; }
        public decimal? Order_Qty { get; set; }
        public decimal? Reported_Qty { get; set; }

        public decimal? UnitPrice { get; set; }
        public string WashDescription { get; set; }
        public string WashType { get; set; }
        public string InvoiceNo { get; set; }
        public decimal? DQty { get; set; }

        public decimal? BFL_in { get; set; }
        public decimal? BFL_out { get; set; }
        public decimal? Dispatch_to_in { get; set; }
        public decimal? Dispatch_to_out { get; set; }
        public decimal? BFL_balance { get; set; }
        public decimal? BFL_onhand { get; set; }
        public decimal? BFL_damage { get; set; }
        public decimal? To_be_recive { get; set; }
        public decimal? Recive_ready { get; set; }

        //Production
        public decimal? Bag_0 { get; set; }
        public decimal? Bag_1 { get; set; }
        public decimal? Bag_2 { get; set; }
        public decimal? Bag_3 { get; set; }
        public decimal? Bag_4 { get; set; }
        public decimal? Bag_5 { get; set; }
        public decimal? Bag_6 { get; set; }

        public decimal? Bag_0_160 { get; set; }
        public decimal? Bag_1_160 { get; set; }
        public decimal? Bag_2_160 { get; set; }
        public decimal? Bag_3_160 { get; set; }
        public decimal? Bag_4_160 { get; set; }
        public decimal? Bag_5_160 { get; set; }
        public decimal? Bag_6_160 { get; set; }

        public decimal? TravelTag_0 { get; set; }
        public decimal? TravelTag_1 { get; set; }
        public decimal? TravelTag_2 { get; set; }
        public decimal? TravelTag_3 { get; set; }
        public decimal? TravelTag_4 { get; set; }
        public decimal? TravelTag_5 { get; set; }
        public decimal? TravelTag_6 { get; set; }
        public decimal? TravelTag_7 { get; set; }
        public decimal? TravelTag_151 { get; set; }
        public decimal? TravelTag_156 { get; set; }
        public decimal? TravelTag_157 { get; set; }
        public decimal? TravelTag_158 { get; set; }
        public decimal? TravelTag_160 { get; set; }


        public decimal? Buddy_Tag_0 { get; set; }
        public decimal? Buddy_Tag_1 { get; set; }
        public decimal? Buddy_Tag_2 { get; set; }
        public decimal? Buddy_Tag_3 { get; set; }
        public decimal? Buddy_Tag_4 { get; set; }
        public decimal? Buddy_Tag_5 { get; set; }
        public decimal? Buddy_Tag_6 { get; set; }
        public decimal? Buddy_Tag_7 { get; set; }
        public decimal? Buddy_Tag_151 { get; set; }
        public decimal? Buddy_Tag_156 { get; set; }
        public decimal? Buddy_Tag_157 { get; set; }
        public decimal? Buddy_Tag_158 { get; set; }
        public decimal? Buddy_Tag_160 { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }

        
        public int JobQty { get; set; }
        public string Weight { get; set; }
        public string PlannedMachine { get; set; }
        public DateTime AllocationDate { get; set; }
        public string EPF { get; set; }

    }
}
