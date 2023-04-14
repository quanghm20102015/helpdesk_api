using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Base
{
    public class BaseRequest
    {
        public string UserId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsTongHop { get; set; }
        public int UnitId { get; set; }
        public int DeparmentId { get; set; }
        public int AccountId { get; set; }
        public string IpAddress { get; set; }
        public int? Role { get; set; }
    }
}
