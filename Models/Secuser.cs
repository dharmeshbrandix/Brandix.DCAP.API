using System;
using System.Collections.Generic;

namespace Brandix.DCAP.API.Models
{
    public partial class Secuser
    {
        public Secuser()
        {
            Secuserright = new HashSet<Secuserright>();
        }

        public string UserId { get; set; }
        public string UserIdN { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public uint? UserType { get; set; }
        public DateTime? LastLogonDateTime { get; set; }
        public uint? UnsuccessfulAttempts { get; set; }
        public DateTime? DateUserActiveFrom { get; set; }
        public DateTime? DateUserExpire { get; set; }
        public DateTime? DatePwdChanged { get; set; }
        public DateTime? DatePasswordExpiry { get; set; }
        public uint Status { get; set; }
        public DateTime? LastAccessedDateTime { get; set; }
        public DateTime? UnsuccessAttemptDateTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedMachine { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedMachine { get; set; }

        public ICollection<Secuserright> Secuserright { get; set; }
    }
}
