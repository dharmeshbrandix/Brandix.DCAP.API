using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class HourlyProduction
    {
        public int TeamId { get; set; }
        public int TeamCode { get; set; }

        public string DayTarget { get; set; }
        public string HourlyTarget { get; set; }
        public string HourName { get; set; }

        public int HourNo01_GDQty { get; set; }
        public int HourNo02_GDQty { get; set; }
        public int HourNo03_GDQty { get; set; }
        public int HourNo04_GDQty { get; set; }
        public int HourNo05_GDQty { get; set; }
        public int HourNo06_GDQty { get; set; }
        public int HourNo07_GDQty { get; set; }
        public int HourNo08_GDQty { get; set; }
        public int HourNo09_GDQty { get; set; }
        public int HourNo10_GDQty { get; set; }
        public int HourNo11_GDQty { get; set; }
        public int HourNo12_GDQty { get; set; }

        public int HourNo01_SCQty { get; set; }
        public int HourNo02_SCQty { get; set; }
        public int HourNo03_SCQty { get; set; }
        public int HourNo04_SCQty { get; set; }
        public int HourNo05_SCQty { get; set; }
        public int HourNo06_SCQty { get; set; }
        public int HourNo07_SCQty { get; set; }
        public int HourNo08_SCQty { get; set; }
        public int HourNo09_SCQty { get; set; }
        public int HourNo10_SCQty { get; set; }
        public int HourNo11_SCQty { get; set; }
        public int HourNo12_SCQty { get; set; }

        public int HourNo01_RWQty { get; set; }
        public int HourNo02_RWQty { get; set; }
        public int HourNo03_RWQty { get; set; }
        public int HourNo04_RWQty { get; set; }
        public int HourNo05_RWQty { get; set; }
        public int HourNo06_RWQty { get; set; }
        public int HourNo07_RWQty { get; set; }
        public int HourNo08_RWQty { get; set; }
        public int HourNo09_RWQty { get; set; }
        public int HourNo10_RWQty { get; set; }
        public int HourNo11_RWQty { get; set; }
        public int HourNo12_RWQty { get; set; }

        public int TotGDQty { get; set; }
        public int TotSCQty { get; set; }
        public int TotRWQty { get; set; }

    }
}