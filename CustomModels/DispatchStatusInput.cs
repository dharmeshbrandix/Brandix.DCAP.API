using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class DispatchStatusInput
    {
        public string ControlId { get; set; }
        public int Seq { get; set; }
        public int DEPId { get; set; }
        public int Status { get; set; }
        public string EnteredBy { get; set; }
        public int Return { get; set; }
    }
}