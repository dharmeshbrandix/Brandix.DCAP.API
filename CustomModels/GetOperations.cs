using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class GetOperationName
    {
        public int? Operation_ID { get; set; }
        public string Operation_Name { get; set; }
    }
}