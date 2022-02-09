using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class GetTeamName
    {
        public uint Team_ID { get; set; }
        public string Team_Name { get; set; }
    }
}