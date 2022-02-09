using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class WFOperations
    {
        public uint WfdepinstId { get; set; }
        public uint Depid { get; set; }
        public string Depdesc { get; set; }
        public string TeamName { get; set; }
        public int? OperationCode { get; set; }
        public string OppTeamName { get; set; }
    }
}
