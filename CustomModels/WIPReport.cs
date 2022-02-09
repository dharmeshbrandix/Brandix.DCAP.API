using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class WIPReport
    {
        public uint L1D { get; set; }
        public string Style { get; set; }
        public uint L2D { get; set; }
        public string Shedule { get; set; }
        public string PO { get; set; }
        public uint L4D { get; set; }
        public string Color { get; set; }
        public uint L5D { get; set; }
        public string Size { get; set; }
        public decimal? Order_Qty { get; set; }

        //Production
        public decimal? M_Laying { get; set; }
        public decimal? M_Cutting { get; set; }
        public decimal? M_EM_1_SQ_PA_Send_PF { get; set; }
        public decimal? M_EM_1_SQ_PA_Receive_PF { get; set; }
        public decimal? M_Sewing_In { get; set; }
        public decimal? M_Sew_Out { get; set; }
        public decimal? M_Washing_Send { get; set; }
        public decimal? M_BFL_Wash_In { get; set; }
        public decimal? M_BFL_Wash_Out { get; set; }
        public decimal? M_Washing_Receive { get; set; }
        public decimal? M_Finishing_In { get; set; }
        public decimal? M_Poly_Bag_Packing { get; set; }
        public decimal? M_Carton_Packing { get; set; }

        //WIP
        public decimal? WIP_Laying { get; set; }
        public decimal? WIP_Cutting { get; set; }
        public decimal? WIP_EM_1_SQ_PA_Send_PF { get; set; }
        public decimal? WIP_EM_1_SQ_PA_Receive_PF { get; set; }
        public decimal? WIP_Sewing_In { get; set; }
        public decimal? WIP_Sew_Out { get; set; }
        public decimal? WIP_Washing_Send { get; set; }
        public decimal? WIP_BFL_Wash_In { get; set; }
        public decimal? WIP_BFL_Wash_Out { get; set; }
        public decimal? WIP_Washing_Receive { get; set; }
        public decimal? WIP_Finishing_In { get; set; }
        public decimal? WIP_Poly_Bag_Packing { get; set; }
        public decimal? WIP_Carton_Packing { get; set; }

    }
}
