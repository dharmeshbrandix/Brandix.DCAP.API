using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class ZWfdepmatrix
    {
        public int Id { get; set; }
        public int Wfid { get; set; }
        public int Depid { get; set; }
        public int WfdepinstId { get; set; }
        public int RowNo { get; set; }
        public int ColNo { get; set; }
        public int TeamId { get; set; }
        public string ColType { get; set; }
        public int WfdepinstIdDisplay { get; set; }
        public int InsStatus { get; set; }
    }
}
