using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class GetFactoryName
    {
        public string Factory_Code { get; set; }
        public string Factory_Name { get; set; }
        public string Loc_Code { get; set; }
        public string Loc_Address { get; set; }
        public string Loc_Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelected { get; set; }
    }
}