using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class RemoveBarcodeChecker
    {
        public uint WFId { get; set; }
        public uint WFDEPInstId { get; set; }
        public uint DEPId { get; set; }
        public uint Seq { get; set; }
        public uint L1idBag { get; set; }
        public uint L2idBag { get; set; }
        public uint L3idBag { get; set; }
        public uint L4idBag { get; set; }
        public uint L5idBag { get; set; }
        public string Barcode { get; set; }
        public string BagBarcode { get; set; }
    }
}