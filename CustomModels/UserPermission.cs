using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public partial class UserPermission
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FunctionId { get; set; }
        public string FuncName { get; set; }
        public string GroupCode { get; set; }
        public string SBUCode { get; set; }
        public string FacCode { get; set; }
        public uint? DEPId { get; set; }
    }
}
