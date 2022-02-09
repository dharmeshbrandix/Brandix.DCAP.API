using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class ComboItemlList
    {
        public string DisplayVal { get; set; }
        public uint WFID { get; set; }
        public uint WfdepinstId { get; set; }
        public uint HourNo { get; set; }
        public string ValueField { get; set; }
    }

    public partial class deplinkfordepinstance
    {
        public uint WFDEPIdLink { get; set; }
        public uint DEPId { get; set; }
        public string DEPDesc { get; set; }

        public string TeamName { get; set; }
        

    }
}
